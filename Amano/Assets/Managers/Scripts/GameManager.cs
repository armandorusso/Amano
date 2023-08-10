using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

public class GameManager : MonoBehaviour
{
    public class EnemyDeathEventArgs : EventArgs
    {
        public int numberOfShuriken { get; set; }
    }

    public EnemyDeathEventArgs enemyDeathEventArgs;
    public static event EventHandler<EnemyDeathEventArgs> EnemyDeathEvent;
    public static Action<int> CollectibleAction;
    public static Action<bool> DeathTrailEffectAction;
    public bool isTetsuoDead { get; private set; }
    public static GameManager Instance { get; private set; }
    [SerializeField] public GameObject GameCanvas;
    [SerializeField] public GameObject GameOverCanvas;
    [SerializeField] private GameObject FKeyUI;
    [SerializeField] private GameObject _tetsuo;
    
    public static int CollectibleCount { get; private set; }
    
    public GameObject CurrentSpawnPoint { get; set; }

    private void Awake()
    {
        // If there is already an instance that exists AND the current instance that is set is not equal to this instance
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else // Set the instance
        {
            Instance = this;
        }
        
        Screen.SetResolution(1920, 1080, true);
        DontDestroyOnLoad(Instance);
    }

    void Start()
    {
        EnemyHealth.EnemyDeathEvent += OnEnemyDeath;
        TetsuoHealthBar.tetsuoDeathEvent += OnTetsuoDeath;
        CollectibleCount = 0;
    }

    private void OnTetsuoDeath(object sender, TetsuoHealthBar.TetsuoDeathEventArgs e)
    {
        if(!isTetsuoDead)
            Invoke(nameof(TetsuoDeathDelay), 0.5f);

        _tetsuo.GetComponent<TeleportAbility>().ReturnAllShuriken();
        
        ChangeTetsuoLayerAndTag("Respawn");
        
        isTetsuoDead = true;
    }

    private void ChangeTetsuoLayerAndTag(string tagName)
    {
        var tetsuoLayers = _tetsuo.GetComponentsInChildren<Transform>();
        
        tetsuoLayers[1].gameObject.layer = LayerMask.NameToLayer(tagName);
        tetsuoLayers[2].gameObject.layer = LayerMask.NameToLayer(tagName);
        tetsuoLayers[4].gameObject.layer = LayerMask.NameToLayer(tagName);
        
        tetsuoLayers[1].gameObject.tag = tagName;
        tetsuoLayers[4].gameObject.tag = tagName;
        
        _tetsuo.gameObject.tag = tagName;
        _tetsuo.gameObject.layer = LayerMask.NameToLayer(tagName);
    }

    private void TetsuoDeathDelay()
    {
        StartCoroutine(MoveTetsuoToCheckpoint());
    }

    private IEnumerator MoveTetsuoToCheckpoint()
    {
        TetsuoDisableMovement.Instance.ResetVelocity();
        // _tetsuo.GetComponent<BoxCollider2D>().enabled = false;
        DeathTrailEffectAction?.Invoke(true);
        
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        
        float currentTime = 0f;
        float duration = 5f;
        
        while (currentTime <= duration && 
               Vector2.Distance(_tetsuo.transform.position, 
                   CurrentSpawnPoint.transform.position) >= 1.5f)
        {
            currentTime += Time.deltaTime;
            _tetsuo.transform.position = Vector3.Lerp(_tetsuo.transform.position,
                CurrentSpawnPoint.transform.position, currentTime / duration);
            yield return null;
        }
        
        isTetsuoDead = false;
        DeathTrailEffectAction?.Invoke(false);
        _tetsuo.GetComponent<BoxCollider2D>().enabled = true;
        _tetsuo.GetComponent<Rigidbody2D>().gravityScale = 3.5f;
        ChangeTetsuoLayerAndTag("Player");
        TetsuoDisableMovement.Instance.EnableOrDisableInputActions(true);
    }

    private void OnEnemyDeath(object sender, EnemyHealth.EnemyDeathEventArgs e)
    {
        var inactiveShurikens = e.enemy.GetComponentsInChildren<ShurikenProjectile>();
        var numberOfShuriken = 0;
        
        Assert.IsNotNull(inactiveShurikens);

        foreach (var shuriken in inactiveShurikens)
        {
            ObjectPool.ObjectPoolInstance.ReturnPooledObject(shuriken.gameObject);
            numberOfShuriken++;
        }
        
        e.enemy.SetActive(false);

        enemyDeathEventArgs = new EnemyDeathEventArgs
        {
            numberOfShuriken = numberOfShuriken
        };
        
        EnemyDeathEvent?.Invoke(this, enemyDeathEventArgs);
    }

    private void OnDestroy()
    {
        EnemyHealth.EnemyDeathEvent -= OnEnemyDeath;
        TetsuoHealthBar.tetsuoDeathEvent -= OnTetsuoDeath;
    }

    public void ReturnAllShuriken()
    {
        _tetsuo.GetComponent<TeleportAbility>().ReturnAllShuriken();
    }

    public void IncrementCollectibleCount()
    {
        CollectibleCount++;
        CollectibleAction?.Invoke(CollectibleCount);
    }
}

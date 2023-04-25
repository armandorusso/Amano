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
    }

    void Start()
    {
        EnemyHealth.EnemyDeathEvent += OnEnemyDeath;
        TetsuoHealthBar.tetsuoDeathEvent += OnTetsuoDeath;
        QuickTimeTeleport.ShowUIArgsEvent += OnQuickTimeEvent;
        CollectibleCount = 0;
    }

    private void OnQuickTimeEvent(object sender, QuickTimeTeleport.ShowUIArgs e)
    {
        FKeyUI.SetActive(e.isActive);
    }

    private void OnTetsuoDeath(object sender, TetsuoHealthBar.TetsuoDeathEventArgs e)
    {
        isTetsuoDead = true;
        Invoke(nameof(TetsuoDeathDelay), 0.5f);
    }

    private void TetsuoDeathDelay()
    {
        StartCoroutine(MoveTetsuoToCheckpoint());
    }

    private IEnumerator MoveTetsuoToCheckpoint()
    {
        TetsuoDisableMovement.Instance.ResetVelocity();
        _tetsuo.GetComponent<BoxCollider2D>().enabled = false;
        
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        
        while (Vector2.Distance(_tetsuo.transform.position, CurrentSpawnPoint.transform.position) >= 1f)
        {
            yield return new WaitForEndOfFrame();
            
            _tetsuo.transform.position = Vector3.Lerp(_tetsuo.transform.position,
                CurrentSpawnPoint.transform.position, Time.deltaTime / 1f * 2f);
        }
        
        TetsuoDisableMovement.Instance.EnableOrDisableInputActions(true);
        isTetsuoDead = false;
        _tetsuo.GetComponent<BoxCollider2D>().enabled = true;
        _tetsuo.GetComponent<Rigidbody2D>().gravityScale = 3.5f;
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
        
        EnemyDeathEvent.Invoke(this, enemyDeathEventArgs);
    }

    private void OnDestroy()
    {
        EnemyHealth.EnemyDeathEvent -= OnEnemyDeath;
        TetsuoHealthBar.tetsuoDeathEvent -= OnTetsuoDeath;
        QuickTimeTeleport.ShowUIArgsEvent -= OnQuickTimeEvent;
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

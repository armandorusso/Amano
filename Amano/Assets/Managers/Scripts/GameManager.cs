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
    public static GameManager Instance { get; private set; }
    [SerializeField] public GameObject GameCanvas;
    [SerializeField] public GameObject GameOverCanvas;
    [SerializeField] private GameObject FKeyUI;
    [SerializeField] private GameObject _tetsuo;
    
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
    }

    private void OnQuickTimeEvent(object sender, QuickTimeTeleport.ShowUIArgs e)
    {
        FKeyUI.SetActive(e.isActive);
    }

    private void OnTetsuoDeath(object sender, TetsuoHealthBar.TetsuoDeathEventArgs e)
    {
        _tetsuo.transform.position = CurrentSpawnPoint.transform.position;
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

    public TeleportAbility GetTeleportAbility()
    {
        return _tetsuo.GetComponent<TeleportAbility>();
    }
}

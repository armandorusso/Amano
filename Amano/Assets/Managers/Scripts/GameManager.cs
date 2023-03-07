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

    [SerializeField] public GameObject GameCanvas;
    [SerializeField] public GameObject GameOverCanvas;
    [SerializeField] private GameObject FKeyUI;
    
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
        GameCanvas.SetActive(false);
        GameOverCanvas.SetActive(true);
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
}

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
    
    void Start()
    {
        EnemyHealth.EnemyDeathEvent += OnEnemyDeath;
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
}

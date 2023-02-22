using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    private float MaxHealthPoints;
    private float CurrentHitPoints;
    
    public class EnemyDeathEventArgs : EventArgs
    {
        public GameObject enemy { get; set; }
    }

    public EnemyDeathEventArgs enemyDeathEventArgs;
    public static event EventHandler<EnemyDeathEventArgs> EnemyDeathEvent;

    private void Start()
    {
        var enemySO = GetComponent<EnemyData>().EnemyParameters;
        MaxHealthPoints = enemySO.Health;
        CurrentHitPoints = MaxHealthPoints;

        ShurikenProjectile.ShurikenHitCharacterEvent += OnEnemyHit;
        QuickTimeTeleport.EnemyDamagedEvent += OnEnemyQuickTimeHit;
    }

    private void OnDisable()
    {
        CurrentHitPoints = MaxHealthPoints;
    }

    private void OnEnemyHit(object sender, ShurikenProjectile.ShurikenHitEventArgs e)
    {
        if (e.enemy == gameObject)
        {
            DamageEnemy(e.damage);
        }
    }

    private void OnEnemyQuickTimeHit(object sender, QuickTimeTeleport.EnemyDamagedEventArgs e)
    {
        if (e.enemy == gameObject)
        {
            DamageEnemy(e.damage);
        }
    }

    private void DamageEnemy(float damage)
    {
        DecreaseHealth(damage);

        if (CurrentHitPoints <= 0)
        {
            enemyDeathEventArgs = new EnemyDeathEventArgs
            {
                enemy = gameObject
            };

            EnemyDeathEvent.Invoke(this, enemyDeathEventArgs);
        }
    }

    private void DecreaseHealth(float damage)
    {
        CurrentHitPoints -= damage;
    }
}

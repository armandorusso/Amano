using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    private float MaxHealthPoints;
    private float CurrentHitPoints;

    private void Start()
    {
        var enemySO = GetComponent<EnemyData>().EnemyParameters;
        MaxHealthPoints = enemySO.Health;
        CurrentHitPoints = MaxHealthPoints;

        ShurikenProjectile.ShurikenHitCharacterEvent += OnEnemyHit;
    }

    private void OnDisable()
    {
        CurrentHitPoints = MaxHealthPoints;
    }

    private void OnEnemyHit(object sender, ShurikenProjectile.ShurikenHitEventArgs e)
    {
        if (e.objectLayer == 7)
        {
            DecreaseHealth(e.damage);

            if (CurrentHitPoints < 0)
            {
                gameObject.SetActive(false);
            }
        }
    }

    private void DecreaseHealth(float damage)
    {
        CurrentHitPoints -= damage;
    }
}

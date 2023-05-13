using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    private float MaxHealthPoints;
    private float CurrentHitPoints;
    private SpriteRenderer _sprite;
    private Color _originalColor;

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
        SlashingHitbox.SlashHitEvent += OnEnemySlashed;
        
        _sprite = GetComponent<SpriteRenderer>();
        _originalColor = _sprite.color;
    }

    private void OnEnemySlashed(object sender, SlashingHitbox.SlashHitEventArgs e)
    {
        if (e.hitGameObject == gameObject)
        {
            DamageEnemy(e.damage);
        }
    }

    private void OnDisable()
    {
        CurrentHitPoints = MaxHealthPoints;
    }

    private void OnEnemyHit(float damage, LayerMask layer, GameObject enemy)
    {
        if (enemy == gameObject && layer == 7)
        {
            DamageEnemy(damage);
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

            EnemyDeathEvent?.Invoke(this, enemyDeathEventArgs);
        }
    }

    private void DecreaseHealth(float damage)
    {
        CurrentHitPoints -= damage;
        StartCoroutine(ChangeSpriteColor());
    }

    private IEnumerator ChangeSpriteColor()
    {
        _sprite.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        _sprite.color = _originalColor;
    }

    private void OnDestroy()
    {
        ShurikenProjectile.ShurikenHitCharacterEvent -= OnEnemyHit;
        SlashingHitbox.SlashHitEvent -= OnEnemySlashed;
    }
}

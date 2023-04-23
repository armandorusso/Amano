using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetsuoHealthBar : MonoBehaviour
{
    // Implement Depencency Inversion properly here
    // The Health class doesnt need to be here. This class should implement the IHealth Interface and the decrease and increase health functions should invoke the events
    // Instead, have the Health class be attached to Tetsuo?
    // To be honest, not sure if we can really apply it here
    private IHealth TetsuoHealthPoints;
    public class HealthUIEventArgs : EventArgs
    {
        public float currentHealth { get; set; }
    }

    public HealthUIEventArgs healthUIEventArgs;
    public static event EventHandler<HealthUIEventArgs> healthUIEvent;
    
    public class TetsuoDeathEventArgs : EventArgs
    {
        public bool isMovementEnabled { get; set; }
    }
    public static event EventHandler<TetsuoDeathEventArgs> tetsuoDeathEvent;

    void Start()
    {
        healthUIEventArgs = new HealthUIEventArgs
        {
            currentHealth = 100f
        };
        TetsuoHealthPoints = new Health(100f);
        healthUIEvent?.Invoke(this, healthUIEventArgs);
        ShurikenProjectile.ShurikenHitCharacterEvent += OnTetsuoDamaged;
        SlashingHitbox.SlashHitEvent += OnMeleeSlashHit;
        DeathZone.TetsuoDeathZoneAction += OnDeathFall;
    }

    private void OnDisable()
    {
        healthUIEventArgs = new HealthUIEventArgs
        {
            currentHealth = 100f
        };
        TetsuoHealthPoints = new Health(100f);
        healthUIEvent?.Invoke(this, healthUIEventArgs);
        
        ShurikenProjectile.ShurikenHitCharacterEvent -= OnTetsuoDamaged;
        SlashingHitbox.SlashHitEvent -= OnMeleeSlashHit;
        DeathZone.TetsuoDeathZoneAction -= OnDeathFall;
    }

    private void OnMeleeSlashHit(object sender, SlashingHitbox.SlashHitEventArgs e)
    {
        if (e.hitGameObject == gameObject)
        {
            TetsuoHealthPoints.DecreaseHealth(e.damage);
            healthUIEventArgs.currentHealth = TetsuoHealthPoints.HitPoints;
            healthUIEvent?.Invoke(this, healthUIEventArgs);
            
            CheckIfTetsuoIsDead();
        }
    }

    private void OnTetsuoDamaged(object sender, ShurikenProjectile.ShurikenHitEventArgs e)
    {
        if (e.objectLayer == 6)
        {
            TetsuoHealthPoints.DecreaseHealth(e.damage);
            healthUIEventArgs.currentHealth = TetsuoHealthPoints.HitPoints;
            healthUIEvent?.Invoke(this, healthUIEventArgs);

            CheckIfTetsuoIsDead();
        }
    }

    private void OnDeathFall(bool disableMovement)
    {
        TetsuoHealthPoints.DecreaseHealth(100f);

        CheckIfTetsuoIsDead();
    }

    private void CheckIfTetsuoIsDead()
    {
        if (TetsuoHealthPoints.HitPoints <= 0)
        {
            TetsuoHealthPoints.IncreaseHealth(100f);
            healthUIEventArgs = new HealthUIEventArgs
            {
                currentHealth = 100f
            };
            
            healthUIEvent?.Invoke(this, healthUIEventArgs);
            
            tetsuoDeathEvent.Invoke(this, new TetsuoDeathEventArgs
            {
                isMovementEnabled = false
            });
        }
    }
}

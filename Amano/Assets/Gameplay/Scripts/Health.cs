using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : IHealth
{
    private float _currentHealth;
    public float HitPoints => _currentHealth;
    public float MaxHealth { get; set; }

    public Health(float health)
    {
        _currentHealth = health;
        MaxHealth = health;
    }
    
    public void IncreaseHealth(float additionalHealthValue)
    {
        _currentHealth += additionalHealthValue;
    }

    public void DecreaseHealth(float damage)
    {
        _currentHealth -= damage;
    }
}

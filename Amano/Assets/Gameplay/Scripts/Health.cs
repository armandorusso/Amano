using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : IHealth
{
    public float HitPoints { get; set; }
    public float MaxHealth { get; set; }

    public Health(float health)
    {
        HitPoints = health;
        MaxHealth = health;
    }
    
    public void IncreaseHealth(float additionalHealthValue)
    {
        HitPoints+= additionalHealthValue;
    }

    public void DecreaseHealth(float damage)
    {
        HitPoints-= damage;
    }
}

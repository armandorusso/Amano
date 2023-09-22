using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealth
{
    public float HitPoints { get; }
    public void SetHealth(float newHealth);
    public void IncreaseHealth(float additionalHealth);
    public void DecreaseHealth(float damage);
}

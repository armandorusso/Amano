using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealth
{
    public float HitPoints { get; }
    public void IncreaseHealth(float additionalHealthValue);
    public void DecreaseHealth(float damage);
}

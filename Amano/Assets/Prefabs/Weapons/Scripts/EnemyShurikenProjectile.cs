using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShurikenProjectile : MonoBehaviour
{
    [SerializeField] public float Damage;

    public static Action<float, LayerMask, GameObject> ShurikenHitCharacterEvent;
    private void OnCollisionEnter2D(Collision2D col)
    {
        GameObject otherObject = col.gameObject;
        
        if (otherObject.layer == 11) return; // Weapons layer
        
        if (otherObject.gameObject.layer is 6) // Player
        {
            ShurikenHitCharacterEvent?.Invoke(Damage, otherObject.gameObject.layer, null);
            ObjectPool.ObjectPoolInstance.ReturnPooledObject(gameObject);
        }
    }
}

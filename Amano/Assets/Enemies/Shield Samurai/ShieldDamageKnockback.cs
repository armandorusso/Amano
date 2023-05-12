using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldDamageKnockback : MonoBehaviour
{
    [SerializeField] private float KnockbackForce;
    [SerializeField] private float ShieldDamage;

    public static Action<float, LayerMask> DamageTetsuoAction;

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            var rb = col.rigidbody;

            col.rigidbody.velocity = new Vector2(col.transform.localScale.x * -1 * KnockbackForce, 10f);
            
            DamageTetsuoAction?.Invoke(ShieldDamage, col.gameObject.layer);
        }
    }
}

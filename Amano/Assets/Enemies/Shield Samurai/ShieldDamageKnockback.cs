using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShieldDamageKnockback : MonoBehaviour
{
    [SerializeField] private float KnockbackForce;
    [SerializeField] private float ShieldDamage;
    [SerializeField] private float DisableMovementDelay;
    [SerializeField] public UnityEvent<float> DisableMovementAction;
    

    public static Action<float, LayerMask> DamageTetsuoAction;

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            var rb = col.rigidbody;
            var direction = (col.transform.position - gameObject.transform.position).normalized;
            
            col.rigidbody.AddForce(new Vector2(direction.x * KnockbackForce, direction.y * 10f), ForceMode2D.Impulse);
            
            DisableMovementAction?.Invoke(DisableMovementDelay);
            
            DamageTetsuoAction?.Invoke(ShieldDamage, col.gameObject.layer);
        }
    }
}

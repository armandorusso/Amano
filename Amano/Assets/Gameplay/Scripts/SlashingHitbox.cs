using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashingHitbox : MonoBehaviour
{
    [SerializeField] public LayerMask HitboxLayerMask;
    [SerializeField] public float Damage;
    
    public class SlashHitEventArgs : EventArgs
    {
        public GameObject hitGameObject { get; set; }
        public float damage { get; set; }
    }

    public SlashHitEventArgs slashHitEventArgs;
    public static event EventHandler<SlashHitEventArgs> SlashHitEvent;

    private void OnTriggerEnter2D(Collider2D col)
    {
        var collidedGameObject = col.gameObject;
        
        if ((HitboxLayerMask.value & 1 << collidedGameObject.layer) != 0)
        {
            slashHitEventArgs = new SlashHitEventArgs
            {
                hitGameObject = collidedGameObject,
                damage = Damage
            };
            
            SlashHitEvent?.Invoke(this, slashHitEventArgs);
        }
    }
}

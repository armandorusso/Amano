using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class ShurikenProjectile : MonoBehaviour
{
    public Animator _animator;
    public Rigidbody2D _rb;
    public Collider2D _collider;
    public TrailRenderer _trailRenderer;
    private bool hitTeleportableObj;
    public static event EventHandler<ShurikenAttachedEventArgs> ShurikenAttachedEvent;

    public class ShurikenAttachedEventArgs : EventArgs
    {
        public bool objectCanTeleport { get; set; }
        public Transform teleportableObject { get; set; }
    }
    public static event EventHandler ShurikenDestroyedEvent;
    
    public static event EventHandler<ShurikenHitEventArgs> ShurikenHitEvent;

    public class ShurikenHitEventArgs : EventArgs
    {
        public ShurikenProjectile shuriken { get; set; }
    }
 
    
    private void Start()
    {
        hitTeleportableObj = false;
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider2D>();
        _trailRenderer = GetComponent<TrailRenderer>();
    }
    
    public void SwitchShurikenProperties(bool isPropertyActive)
    {
        _animator.enabled = isPropertyActive;
        _collider.enabled = isPropertyActive;
        _rb.simulated = isPropertyActive;
        _trailRenderer.enabled = isPropertyActive;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        GameObject otherObject = col.gameObject;

        if (otherObject.layer == 8 || otherObject.layer == 9 || otherObject.layer == 10)
        {
            Assert.IsNotNull(_rb);
            Assert.IsNotNull(_animator);
            Assert.IsNotNull(_collider);
            
            _animator.enabled = false;
            _trailRenderer.emitting = false;
            _rb.simulated = false;
            _collider.enabled = false;
            ShurikenHitEventArgs arg = new ShurikenHitEventArgs
            {
                shuriken = this
            };
            
            ShurikenHitEvent.Invoke(this, arg);

            if (otherObject.CompareTag("Teleport"))
            {
                transform.parent = otherObject.transform;
                hitTeleportableObj = true;
                
                ShurikenAttachedEventArgs args = new ShurikenAttachedEventArgs
                {
                    objectCanTeleport = hitTeleportableObj,
                    teleportableObject = otherObject.transform,
                };
                ShurikenAttachedEvent.Invoke(this, args);
                hitTeleportableObj = false;
            }
        }
    }
}

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
    private bool hitGroundOrWall;
    private bool _instantiatedShurikenProperties = false;
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
        hitGroundOrWall = false;
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider2D>();
        _trailRenderer = GetComponent<TrailRenderer>();
        _instantiatedShurikenProperties = true;
    }

    public void OnEnable()
    {
        if(_instantiatedShurikenProperties)
            SwitchShurikenProperties(true);
    }

    public void OnDisable()
    {
        if (_instantiatedShurikenProperties)
        {
            SwitchShurikenProperties(false);
            hitGroundOrWall = false;
        }
    }

    private void SwitchShurikenProperties(bool b)
    {
        _animator.enabled = b;
        _collider.enabled = b;
        _rb.simulated = b;
        _trailRenderer.enabled = b;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        GameObject otherObject = col.gameObject;

        if (!hitGroundOrWall && (otherObject.layer == 8 || otherObject.layer == 9 || otherObject.layer == 10))
        {
            hitGroundOrWall = true;
            Assert.IsNotNull(_rb);
            Assert.IsNotNull(_animator);
            Assert.IsNotNull(_collider);
            
            SwitchShurikenProperties(false);

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
            else
            {
                ShurikenHitEventArgs args = new ShurikenHitEventArgs
                {
                    shuriken = this
                };
            
                ShurikenHitEvent.Invoke(this, args);
            }
        }
    }
}

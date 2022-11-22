using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class ShurikenProjectile : MonoBehaviour
{
    private Animator _animator;
    private Rigidbody2D _rb;
    private Collider2D _collider;
    private bool hitTeleportableObj;
    public static event EventHandler<ShurikenSpawnedEventArgs> ShurikenSpawnedEvent;

    public class ShurikenSpawnedEventArgs : EventArgs
    {
        public GameObject shuriken { get; set; }
    }
    public static event EventHandler<ShurikenAttachedEventArgs> ShurikenAttachedEvent;

    public class ShurikenAttachedEventArgs : EventArgs
    {
        public bool objectCanTeleport { get; set; }
        public Transform teleportableObject { get; set; }
    }
    public static event EventHandler ShurikenDestroyedEvent;
 
    
    private void Start()
    {
        hitTeleportableObj = false;
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider2D>();
        ShurikenSpawnedEventArgs args = new ShurikenSpawnedEventArgs
        {
            shuriken = gameObject
        };
        ShurikenSpawnedEvent.Invoke(this, args);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        GameObject otherObject = col.gameObject;

        if (otherObject.layer == 8 || otherObject.layer == 9)
        {
            Assert.IsNotNull(_rb);
            Assert.IsNotNull(_animator);
            Assert.IsNotNull(_collider);
            
            _animator.enabled = false;
            Destroy(_rb);
            _collider.enabled = false;

            if (otherObject.CompareTag("Teleport"))
            {
                transform.parent = otherObject.transform;
                hitTeleportableObj = true;
                
                ShurikenAttachedEventArgs args = new ShurikenAttachedEventArgs
                {
                    objectCanTeleport = hitTeleportableObj,
                    teleportableObject = otherObject.transform
                };
                ShurikenAttachedEvent.Invoke(this, args);
                hitTeleportableObj = false;
            }
        }
    }
}

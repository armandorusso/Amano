using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class ShurikenProjectile : MonoBehaviour
{
    [SerializeField] public float Damage;
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
        public bool quickTimeTeleport { get; set; }
        public GameObject enemy { get; set; }
    }
    public static event EventHandler ShurikenDestroyedEvent;
    
    public static event EventHandler<ShurikenHitEventArgs> ShurikenHitEvent;
    public static event EventHandler<ShurikenHitEventArgs> ShurikenHitCharacterEvent;

    public class ShurikenHitEventArgs : EventArgs
    {
        public float damage { get; set; }
        public LayerMask objectLayer { get; set; }
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
        SwitchShurikenProperties(false);
        
        if (otherObject.layer == 11) return; // Weapons layer
        
        if (otherObject.gameObject.layer is 6) // Player
        {
            ShurikenHitEventArgs args = new ShurikenHitEventArgs
            {
                shuriken = this,
                damage = Damage,
                objectLayer = otherObject.gameObject.layer
            };

            ShurikenHitCharacterEvent.Invoke(this, args);
            ObjectPool.ObjectPoolInstance.ReturnPooledObject(gameObject);

            return;
        }
        
        if (!hitGroundOrWall && otherObject.layer is 7 or 8 or 9 or 10) // Enemy, Ground, Wall, or Item
        {

            hitGroundOrWall = true;
            Assert.IsNotNull(_rb);
            Assert.IsNotNull(_animator);
            Assert.IsNotNull(_collider);

            if (otherObject.CompareTag("Teleport"))
            {
                transform.parent = otherObject.transform;
                hitTeleportableObj = true;

                var success = otherObject.TryGetComponent(out EnemyData quickTimeComponent);
                GameObject teleportObject = null;
                int? teleportObjectLayer = null;

                if (success)
                {
                    teleportObject = quickTimeComponent.gameObject.transform.GetChild(0).GetChild(0).gameObject;
                    teleportObjectLayer = teleportObject.gameObject.layer;
                }


                ShurikenAttachedEventArgs args = new ShurikenAttachedEventArgs
                {
                    objectCanTeleport = hitTeleportableObj,
                    teleportableObject = teleportObjectLayer == 13 ? teleportObject.transform : otherObject.transform,
                    quickTimeTeleport = teleportObjectLayer == 13 ? teleportObject.transform : null, // if doesn't exist, false
                    enemy = otherObject
                };
                
                ShurikenAttachedEvent.Invoke(this, args);
                hitTeleportableObj = false;

                if (teleportObject)
                    return;
            }
            else
            {
                ShurikenHitEventArgs args = new ShurikenHitEventArgs
                {
                    shuriken = this
                };

                ShurikenHitEvent.Invoke(this, args);
            }
            
            if (otherObject.layer == 7) // Enemy
            {
                ShurikenHitEventArgs args = new ShurikenHitEventArgs
                {
                    shuriken = this,
                    damage = Damage,
                    objectLayer = otherObject.gameObject.layer
                };
                
                ShurikenHitCharacterEvent.Invoke(this, args);
            }
        }
    }
}

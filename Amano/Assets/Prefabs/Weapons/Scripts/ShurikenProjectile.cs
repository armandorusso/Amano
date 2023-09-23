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
    [SerializeField] public Color ShurikenAttachedColor;
    [SerializeField] public ParticleSystem ShurikenParticleSystem;
    public SpriteRenderer _sprite;
    public Animator _animator;
    public Rigidbody2D _rb;
    public Collider2D _collider;
    public TrailRenderer _trailRenderer;
    private Color _originalColor;
    private bool hitTeleportableObj;
    private bool hitGroundOrWall;
    private bool _instantiatedShurikenProperties = false;
    
    public static event EventHandler<ShurikenAttachedEventArgs> ShurikenAttachedEvent;
    // TODO: Add UI Button Sprite to the shuriken indicating that it is attached to a teleportable item
    // TODO: Add a sound effect for when it attaches to a teleportable object

    public class ShurikenAttachedEventArgs : EventArgs
    {
        public bool objectCanTeleport { get; set; }
        public Transform teleportableObject { get; set; }
        public bool quickTimeTeleport { get; set; }
        public GameObject enemy { get; set; }
    }
    public static event EventHandler ShurikenDestroyedEvent;
    
    public static event EventHandler<ShurikenHitEventArgs> ShurikenHitEvent;
    public static Action<float, LayerMask, GameObject> ShurikenHitCharacterEvent;
    public static Action <int, GameObject> ShurikenHitTeleportObjectAction;

    public class ShurikenHitEventArgs : EventArgs
    {
        public float damage { get; set; }
        public LayerMask objectLayer { get; set; }
        public ShurikenProjectile shuriken { get; set; }
        public GameObject enemy { get; set; }
    }

    private void Awake()
    {
        hitTeleportableObj = false;
        hitGroundOrWall = false;
        _rb = GetComponent<Rigidbody2D>();
        _sprite = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider2D>();
        _trailRenderer = GetComponent<TrailRenderer>();
        _instantiatedShurikenProperties = true;
        _originalColor = _sprite.color;
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

    private void SwitchShurikenProperties(bool isEnabled)
    {
        _animator.enabled = isEnabled;
        _collider.enabled = isEnabled;
        _rb.simulated = isEnabled;
        _trailRenderer.enabled = isEnabled;
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

            ShurikenHitCharacterEvent?.Invoke(Damage, otherObject.gameObject.layer, null);
            ObjectPool.ObjectPoolInstance.ReturnPooledObject(gameObject);

            return;
        }
        
        if (!hitGroundOrWall && otherObject.layer is 7 or 8 or 9 or 10 or 13) // Enemy, Ground, Wall, Item or QuickTimeEvent
        {
            transform.parent = otherObject.transform;
            hitGroundOrWall = true;
            Assert.IsNotNull(_rb);
            Assert.IsNotNull(_animator);
            Assert.IsNotNull(_collider);

            if (otherObject.CompareTag("Teleport"))
            {
                ShurikenHitTeleportObjectAction?.Invoke(gameObject.GetInstanceID(), gameObject);
                var contactedCollider = col.GetContact(0).otherCollider;
                if (contactedCollider != null && contactedCollider.gameObject.layer is 10)
                {
                    otherObject = contactedCollider.gameObject;
                }
                
                hitTeleportableObj = true;

                GameObject teleportObject = null;
                int? teleportObjectLayer = null;
                
                ShurikenAttachedEventArgs args = new ShurikenAttachedEventArgs
                {
                    objectCanTeleport = hitTeleportableObj,
                    teleportableObject = teleportObjectLayer == 13 ? teleportObject.transform : otherObject.transform,
                    quickTimeTeleport = teleportObjectLayer == 13 ? teleportObject.transform : false, // if doesn't exist, false
                    enemy = otherObject
                };
                
                ShurikenAttachedEvent?.Invoke(this, args);
                hitTeleportableObj = false;
            }
            else // TODO: Don't depend on certain layers to return shuriken into the pool. Some objects don't have specific layers so the shuriken don't return
            {
                ShurikenHitEventArgs args = new ShurikenHitEventArgs
                {
                    shuriken = this
                };

                ShurikenHitEvent?.Invoke(this, args);
            }
            
            if (otherObject.layer == 7) // Enemy
            {
                otherObject.TryGetComponent(out EnemyHealth enemy);
                ShurikenHitEventArgs args = new ShurikenHitEventArgs
                {
                    shuriken = this,
                    damage = Damage,
                    objectLayer = otherObject.gameObject.layer,
                    enemy = enemy.gameObject
                };
                
                ShurikenHitCharacterEvent?.Invoke(Damage, otherObject.gameObject.layer, enemy.gameObject);
            }
        }
    }
}

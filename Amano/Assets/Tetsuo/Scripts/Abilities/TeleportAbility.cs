using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class TeleportAbility : MonoBehaviour
{
    private Queue<ShurikenProjectile> _normalShurikens;
    private Queue<ShurikenProjectile> _teleportShurikens;
    private Queue<Transform> _teleportableObjects;
    private bool canTeleport;
    private Rigidbody2D _rb;
    
    public class VanishingFxEventArgs : EventArgs
    {
        public GameObject objectBeingTeleported1 { get; set; }
        public GameObject objectBeingTeleported2 { get; set; }
    }
    public static event EventHandler<VanishingFxEventArgs> VanishingEvent;
    private VanishingFxEventArgs vanishingEventArgs;
    
    public class QuickTimeTeleportEventArgs : EventArgs
    {
        public GameObject objectBeingTeleported { get; set; }
        public GameObject enemy { get; set; }
    }
    public static event EventHandler<QuickTimeTeleportEventArgs> QuickTimeTeleportEvent;
    private QuickTimeTeleportEventArgs quickTimeTeleportEventArgs;

    void Awake()
    {
        ShurikenProjectile.ShurikenHitEvent += OnShurikenHitEvent;
        ShurikenProjectile.ShurikenAttachedEvent += OnShurikenAttachedEvent;

        vanishingEventArgs = new VanishingFxEventArgs
        {
            objectBeingTeleported1 = null,
            objectBeingTeleported2 = null
        };
    }

    private void Start()
    {
        _normalShurikens = new Queue<ShurikenProjectile>(10);
        _teleportShurikens = new Queue<ShurikenProjectile>(10);
        _teleportableObjects = new Queue<Transform>(10);
        canTeleport = false;
        _rb = GetComponent<Rigidbody2D>();

        GameManager.EnemyDeathEvent += OnEnemyDefeated;
    }

    private void OnShurikenHitEvent(object sender, ShurikenProjectile.ShurikenHitEventArgs e)
    {
        Debug.Log("Event invoked");

        if (sender is ShurikenProjectile shurikenProjectile)
        {
            _normalShurikens.Enqueue(shurikenProjectile);
            
            if (_normalShurikens.Count > 5)
            {
                var shurikenToRemove = _normalShurikens.Dequeue();
                
                ObjectPool.ObjectPoolInstance.ReturnPooledObject(shurikenToRemove.gameObject);
            }
        }
    }

    private void OnShurikenAttachedEvent(object sender, ShurikenProjectile.ShurikenAttachedEventArgs e)
    {
        canTeleport = e.objectCanTeleport;

        if (sender is ShurikenProjectile shurikenProjectile)
        {
            _teleportShurikens.Enqueue(shurikenProjectile);
            _teleportableObjects.Enqueue(e.teleportableObject);

            if (_teleportShurikens.Count > 5)
            {
                var shuriken = _teleportShurikens.Dequeue();
                _teleportableObjects.Dequeue();
                
                ObjectPool.ObjectPoolInstance.ReturnPooledObject(shuriken.gameObject);
            }
        }
    }

    private void OnEnemyDefeated(object sender, GameManager.EnemyDeathEventArgs e)
    {
        for (int i = 0; i < e.numberOfShuriken; i++)
        {
            _teleportShurikens.Dequeue();
            _teleportableObjects.Dequeue();
        }
    }

    public void TeleportToObject(InputAction.CallbackContext context)
    {
        if (context.performed && canTeleport && _teleportShurikens.Count > 0)
        {
            var shuriken = _teleportShurikens.Peek();
            Debug.Log("Swapping positions");
            _teleportShurikens.Dequeue();
            var objectToTeleport = _teleportableObjects.Dequeue();
            Assert.IsNotNull(shuriken);
            vanishingEventArgs.objectBeingTeleported1 = gameObject;
            vanishingEventArgs.objectBeingTeleported2 = objectToTeleport.gameObject;
            VanishingEvent.Invoke(this, vanishingEventArgs);
            var playerPosition = gameObject.transform.position;
            gameObject.transform.position = objectToTeleport.position;
            objectToTeleport.transform.position = playerPosition;

            ObjectPool.ObjectPoolInstance.ReturnPooledObject(shuriken.gameObject);

            if (objectToTeleport.gameObject.layer == 13)
            {
                quickTimeTeleportEventArgs = new QuickTimeTeleportEventArgs
                {
                    objectBeingTeleported = objectToTeleport.gameObject,
                    enemy = objectToTeleport.transform.parent.parent.gameObject
                };

                QuickTimeTeleportEvent?.Invoke(this, quickTimeTeleportEventArgs);
            }

            if (_teleportShurikens.Count < 1)
            {
                canTeleport = false;
            }
        }
    }

    public void ReturnAllShuriken()
    {
        while(_normalShurikens.Count != 0)
        {
            ObjectPool.ObjectPoolInstance.ReturnPooledObject(_normalShurikens.Dequeue().gameObject);
        }
        
        while(_teleportShurikens.Count != 0)
        {
            ObjectPool.ObjectPoolInstance.ReturnPooledObject(_teleportShurikens.Dequeue().gameObject);
        }

        while (_teleportableObjects.Count != 0)
        {
            _teleportableObjects.Dequeue();
        }
    }

    private void OnDestroy()
    {
        ShurikenProjectile.ShurikenHitEvent -= OnShurikenHitEvent;
        ShurikenProjectile.ShurikenAttachedEvent -= OnShurikenAttachedEvent;
        GameManager.EnemyDeathEvent -= OnEnemyDefeated;
    }
}

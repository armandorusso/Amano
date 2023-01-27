using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class TeleportAbility : MonoBehaviour
{
    private Queue<ShurikenProjectile> _shurikens;
    private Queue<ShurikenProjectile> _teleportShurikens;
    private Queue<Transform> _teleportableObjects;
    private bool canTeleport;
    
    public class VanishingFxEventArgs : EventArgs
    {
        public GameObject objectBeingTeleported1 { get; set; }
        public GameObject objectBeingTeleported2 { get; set; }
    }
    public static event EventHandler<VanishingFxEventArgs> VanishingEvent;
    private VanishingFxEventArgs vanishingEventArgs;

    void Awake()
    {
        ShurikenProjectile.ShurikenHitEvent += OnShurikenHitEvent;
        ShurikenProjectile.ShurikenAttachedEvent += OnShurikenAttachedEvent;

        vanishingEventArgs = new VanishingFxEventArgs()
        {
            objectBeingTeleported1 = null,
            objectBeingTeleported2 = null
        };
    }

    private void Start()
    {
        _shurikens = new Queue<ShurikenProjectile>(10);
        _teleportShurikens = new Queue<ShurikenProjectile>(10);
        _teleportableObjects = new Queue<Transform>(10);
        canTeleport = false;
    }

    // Not used right now
    private void OnShurikenHitEvent(object sender, ShurikenProjectile.ShurikenHitEventArgs e)
    {
        Debug.Log("Event invoked");

        if (sender is ShurikenProjectile shurikenProjectile)
        {
            _shurikens.Enqueue(shurikenProjectile);
            
            if (_shurikens.Count > 5)
            {
                var shurikenToRemove = _shurikens.Dequeue();

                if (_teleportShurikens.Count > 0 && shurikenToRemove.GetInstanceID() == _teleportShurikens.Peek().GetInstanceID())
                {
                    _teleportShurikens.Dequeue();
                    _teleportableObjects.Dequeue();
                }

                ObjectPool.ObjectPoolInstance.ReturnPooledObject(shurikenToRemove.gameObject);
                shurikenToRemove.SwitchShurikenProperties(true);
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
            shurikenProjectile.SwitchShurikenProperties(false);

            if (_teleportShurikens.Count > 5)
            {
                var shuriken = _teleportShurikens.Dequeue();
                _teleportableObjects.Dequeue();
                if (_shurikens.Count > 0 && _shurikens.Peek().GetInstanceID() == shuriken.GetInstanceID())
                {
                    _shurikens.Dequeue();
                }
                ObjectPool.ObjectPoolInstance.ReturnPooledObject(shuriken.gameObject);
                shuriken.SwitchShurikenProperties(true);
            }
        }
    }

    public void TeleportToObject(InputAction.CallbackContext context)
    {
        Debug.Log("Swapping positions");
        if (context.performed && canTeleport && _teleportShurikens.Count > 0)
        {
            var shuriken = _teleportShurikens.Dequeue();
            var objectToTeleport = _teleportableObjects.Dequeue();
            Assert.IsNotNull(shuriken);
            vanishingEventArgs.objectBeingTeleported1 = gameObject;
            vanishingEventArgs.objectBeingTeleported2 = objectToTeleport.gameObject;
            VanishingEvent.Invoke(this, vanishingEventArgs);
            var playerPosition = gameObject.transform.position;
            gameObject.transform.position = objectToTeleport.position;
            objectToTeleport.transform.position = playerPosition;
            if (_shurikens.Count > 0)
            {
                while (_shurikens.Peek().GetInstanceID() != shuriken.GetInstanceID())
                {
                    var obj = _shurikens.Dequeue();
                    _shurikens.Enqueue(obj);
                }

                _shurikens.Dequeue();
            }
            
            ObjectPool.ObjectPoolInstance.ReturnPooledObject(shuriken.gameObject);
            shuriken.SwitchShurikenProperties(true);
        }

        if (_teleportShurikens.Count < 1)
        {
            canTeleport = false;
        }
    }
}

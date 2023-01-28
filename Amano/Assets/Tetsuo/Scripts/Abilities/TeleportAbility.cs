using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class TeleportAbility : MonoBehaviour
{
    private Queue<ShurikenProjectile> _normalShurikens;
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
        _normalShurikens = new Queue<ShurikenProjectile>(10);
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
            _normalShurikens.Enqueue(shurikenProjectile);
            
            if (_normalShurikens.Count > 5)
            {
                var shurikenToRemove = _normalShurikens.Dequeue();

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

            ObjectPool.ObjectPoolInstance.ReturnPooledObject(shuriken.gameObject);
            shuriken.SwitchShurikenProperties(true);
        }

        if (_teleportShurikens.Count < 1)
        {
            canTeleport = false;
        }
    }
}

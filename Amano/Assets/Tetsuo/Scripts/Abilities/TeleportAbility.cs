using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class TeleportAbility : MonoBehaviour
{
    private List<ShurikenProjectile> _shurikens;
    private Queue<ShurikenProjectile> _teleportShurikens;
    private Queue<Transform> _teleportableObjects;
    private bool canTeleport;

    void Awake()
    {
        ShurikenProjectile.ShurikenSpawnedEvent += OnShurikenSpawnedEvent;
        ShurikenProjectile.ShurikenAttachedEvent += OnShurikenAttachedEvent;
    }

    private void Start()
    {
        _shurikens = new List<ShurikenProjectile>(10);
        _teleportShurikens = new Queue<ShurikenProjectile>(10);
        _teleportableObjects = new Queue<Transform>(10);
        canTeleport = false;
    }

    // Not used right now
    private void OnShurikenSpawnedEvent(object sender, ShurikenProjectile.ShurikenSpawnedEventArgs e)
    {
        Debug.Log("Event invoked");
        
        if(sender is ShurikenProjectile shurikenProjectile) 
            _shurikens.Add(shurikenProjectile);
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
                Destroy(shuriken.gameObject);
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
            var playerPosition = gameObject.transform.position;
            gameObject.transform.position = objectToTeleport.position;
            objectToTeleport.transform.position = playerPosition;
            shuriken.gameObject.SetActive(false);
        }

        if (_teleportShurikens.Count < 1)
        {
            canTeleport = false;
        }
    }
}

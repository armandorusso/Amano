using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TeleportAbility : MonoBehaviour
{
    private List<ShurikenProjectile> _shurikens;
    private bool canTeleport;
    private Transform teleportableObject;

    void Awake()
    {
        ShurikenProjectile.ShurikenSpawnedEvent += OnShurikenSpawnedEvent;
        ShurikenProjectile.ShurikenAttachedEvent += OnShurikenAttachedEvent;
    }

    private void Start()
    {
        _shurikens = new List<ShurikenProjectile>(10);
        canTeleport = false;
    }

    private void OnShurikenSpawnedEvent(object sender, ShurikenProjectile.ShurikenSpawnedEventArgs e)
    {
        Debug.Log("Event invoked");
        
        if(sender is ShurikenProjectile shurikenProjectile) 
            _shurikens.Add(shurikenProjectile);
    }
    
    private void OnShurikenAttachedEvent(object sender, ShurikenProjectile.ShurikenAttachedEventArgs e)
    {
        canTeleport = e.objectCanTeleport;
        teleportableObject = e.teleportableObject;
    }

    public void TeleportToObject(InputAction.CallbackContext context)
    {
        Debug.Log("Swapping positions");
        if (context.performed && canTeleport && _shurikens.Count > 0)
        {
            var shuriken = _shurikens[0];
            var playerPosition = gameObject.transform.position;
            gameObject.transform.position = teleportableObject.position;
            teleportableObject.transform.position = playerPosition;
        }
    }
}

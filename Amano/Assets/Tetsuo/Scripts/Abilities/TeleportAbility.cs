using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TeleportAbility : MonoBehaviour
{
    private List<ShurikenProjectile> _shurikens;
    private List<ShurikenProjectile> _teleportShurikens;
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
        _teleportShurikens = new List<ShurikenProjectile>(10);
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
        if(sender is ShurikenProjectile shurikenProjectile) 
            _teleportShurikens.Add(shurikenProjectile);
    }

    public void TeleportToObject(InputAction.CallbackContext context)
    {
        Debug.Log("Swapping positions");
        if (context.performed && canTeleport && _teleportShurikens.Count > 0)
        {
            var shuriken = _teleportShurikens[0];
            var playerPosition = gameObject.transform.position;
            gameObject.transform.position = teleportableObject.position;
            teleportableObject.transform.position = playerPosition;
            Destroy(shuriken.gameObject);
            _teleportShurikens.RemoveAt(0);
        }

        if (_teleportShurikens.Count < 1)
        {
            canTeleport = false;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class RemoveShieldComponents : MonoBehaviour
{
    private void Start()
    {
        TeleportAbility.RemoveShieldComponentsAction += OnTeleport;
    }

    private void OnTeleport(ShieldRotation shield)
    {
        shield.transform.parent = null;
        Destroy(shield.GetComponent<ShieldRotation>());
        TeleportAbility.RemoveShieldComponentsAction -= OnTeleport;
    }

    private void OnDisable()
    {
        TeleportAbility.RemoveShieldComponentsAction -= OnTeleport;
    }
}

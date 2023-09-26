using System;
using Unity.VisualScripting;
using UnityEngine;

public class RemoveShieldComponents : MonoBehaviour
{
    private bool _hasRemoved;
    
    private void Start()
    {
        TeleportAbility.RemoveShieldComponentsAction += OnTeleport;
    }

    private void OnTeleport(ShieldRotation shield)
    {
        if (!_hasRemoved && shield.gameObject.GetInstanceID() == gameObject.GetInstanceID())
        {
            // Set the shield's parent to Room Components since we want it to be disabled when Tetsuo leaves the room
            var roomComponentsTransform = shield.transform.parent.gameObject.transform.parent.gameObject.transform.parent;
            shield.transform.parent = roomComponentsTransform;

            if (shield.TryGetComponent(out ShieldRotation shieldRotationComponent))
            {
                shieldRotationComponent.enabled = false;
            }

            shield.AddComponent<Rigidbody2D>();

            TeleportAbility.RemoveShieldComponentsAction -= OnTeleport;

            _hasRemoved = true;
        }
    }

    private void OnEnable()
    {
        TeleportAbility.RemoveShieldComponentsAction += OnTeleport;
    }

    private void OnDisable()
    {
        TeleportAbility.RemoveShieldComponentsAction -= OnTeleport;
        _hasRemoved = false;
    }
}

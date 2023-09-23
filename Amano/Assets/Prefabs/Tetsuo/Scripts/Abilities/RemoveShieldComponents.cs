using Unity.VisualScripting;
using UnityEngine;

public class RemoveShieldComponents : MonoBehaviour
{
    private void Start()
    {
        TeleportAbility.RemoveShieldComponentsAction += OnTeleport;
    }

    private void OnTeleport(ShieldRotation shield)
    {
        shield.transform.parent = null;
        
        if(shield.TryGetComponent(out ShieldRotation shieldRotationComponent))
        {
            shieldRotationComponent.enabled = false;
        }

        // shield.AddComponent<Rigidbody2D>();
        
        TeleportAbility.RemoveShieldComponentsAction -= OnTeleport;
    }

    private void OnDisable()
    {
        TeleportAbility.RemoveShieldComponentsAction -= OnTeleport;
    }
}

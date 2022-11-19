using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
 
public class MousePosTracker : MonoBehaviour
{
    [SerializeField] private Camera camera;
    private Vector3 mousePositionInWorld { get; set; }
    
    public Vector3 GetMousePositionInWorld()
    {
        return mousePositionInWorld;
    }

    public void OnShootWeapon(InputAction.CallbackContext context)
    {
        mousePositionInWorld = camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    }
        
}

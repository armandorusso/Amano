using UnityEngine;
using UnityEngine.InputSystem;
 
public class AimDirectionTracker : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] public bool usingController;
    private Vector3 mousePositionInWorld { get; set; }
    private Vector2 rightStickDirection { get; set; }
    
    public Vector3 GetMousePositionInWorld()
    {
        return mousePositionInWorld;
    }

    public Vector2 GetRightStickDirection()
    {
        return rightStickDirection;
    }

    public void OnShootWeapon(InputAction.CallbackContext context)
    {
        if(!usingController)
            mousePositionInWorld = camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        else
        {
            if (context.started || context.performed)
            {
                Debug.Log($"Controller input {rightStickDirection}");
                rightStickDirection = context.ReadValue<Vector2>();
            }
        }
    }
        
}

using System;
using UnityEngine;
using UnityEngine.InputSystem;
 
public class AimDirectionTracker : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] public GameInputManager.InputType CurrentInput;
    private Vector3 mousePositionInWorld { get; set; }
    private Vector2 rightStickDirection { get; set; }

    private void Start()
    {
        GameInputManager.SwitchInputAction += OnControllerInputSwitch;
    }

    private void OnControllerInputSwitch(GameInputManager.InputType isUsingController)
    {
        CurrentInput = isUsingController;
    }

    private void Update()
    {
    }

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
        if(CurrentInput == GameInputManager.InputType.KeyboardMouse)
            mousePositionInWorld = camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        else
        {
            if (context.started || context.performed)
            {
                rightStickDirection = context.ReadValue<Vector2>();
            }
        }
    }
        
}

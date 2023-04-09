using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInputManager : MonoBehaviour
{
    public enum InputType
    {
        KeyboardMouse,
        Controller
    }

    [SerializeField] public InputAction _controllerInput;
    [SerializeField] public InputAction _keyboardAndMouseInput;
    public InputType currentInputType;
    public float inputSwitchCooldown = 0.5f;
    private float lastInputSwitchTime;

    private void Start()
    {
        _controllerInput.Enable();
        _keyboardAndMouseInput.Enable();
    }

    private void Update()
    {
        DetectInputType();
    }

    private void DetectInputType()
    {
        // Check for keyboard and mouse inputs
        if (_keyboardAndMouseInput.IsInProgress())
        {
            if (currentInputType != InputType.KeyboardMouse && Time.time - lastInputSwitchTime > inputSwitchCooldown)
            {
                currentInputType = InputType.KeyboardMouse;
                lastInputSwitchTime = Time.time;
                Debug.Log("Using Keyboard and Mouse");
            }
        }

        // Check for controller inputs
        if (_controllerInput.IsInProgress())
        {
            if (currentInputType != InputType.Controller && Time.time - lastInputSwitchTime > inputSwitchCooldown)
            {
                currentInputType = InputType.Controller;
                lastInputSwitchTime = Time.time;
                Debug.Log("Using Controller");
            }
        }
    }
}

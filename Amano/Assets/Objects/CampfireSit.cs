using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class CampfireSit : MonoBehaviour
{
    [SerializeField] public UnityEvent SittingEvent;
    [SerializeField] public UnityEvent LoadNewLevelEvent;
    [SerializeField] public InputAction SittingButton;
    [SerializeField] public SpriteRenderer InteractButton;
    [SerializeField] public Sprite InteractButtonKeyboard;
    [SerializeField] public Sprite InteractButtonController;

    private bool hasPressedCampfireButton;

    private void Start()
    {
        GameInputManager.SwitchInputAction += OnSwitchInput;
    }

    private void Update()
    {
        hasPressedCampfireButton = Input.GetKeyDown(KeyCode.F) || SittingButton.IsInProgress();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            InteractButton.enabled = true;
            
            if (hasPressedCampfireButton)
            {
                SittingEvent?.Invoke();
                LoadNewLevelEvent?.Invoke();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            InteractButton.enabled = false;
        }
    }
    
    private void OnSwitchInput(GameInputManager.InputType inputType)
    {
        if (inputType == GameInputManager.InputType.Controller)
        {
            InteractButton.sprite = InteractButtonController;
        }
        else
        {
            InteractButton.sprite = InteractButtonKeyboard;
        }
    }
}

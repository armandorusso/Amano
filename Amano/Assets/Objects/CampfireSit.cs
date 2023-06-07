using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class CampfireSit : MonoBehaviour
{
    [SerializeField] public UnityEvent SittingEvent;
    [SerializeField] public UnityEvent LoadNewLevelEvent;
    [SerializeField] public InputAction SittingButton;

    private bool hasPressedCampfireButton;

    private void Update()
    {
        hasPressedCampfireButton = Input.GetKeyDown(KeyCode.F) || SittingButton.IsInProgress();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            if (hasPressedCampfireButton)
            {
                SittingEvent?.Invoke();
                LoadNewLevelEvent?.Invoke();
            }
        }
    }
}

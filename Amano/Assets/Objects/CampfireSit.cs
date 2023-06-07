using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class CampfireSit : MonoBehaviour
{
    [SerializeField] public UnityEvent SittingEvent;
    [SerializeField] public UnityEvent LoadNewLevelEvent;
    [SerializeField] public InputAction SittingButton;


    private void OnTriggerStay2D(Collider2D other)
    {
        if (SittingButton.IsPressed())
        {
            SittingEvent?.Invoke();
            LoadNewLevelEvent?.Invoke();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TetsuoZoom : MonoBehaviour
{
    [SerializeField] public AimDirectionTracker AimTracker;
    private bool _isZoomActive;

    public static Action<bool, AimDirectionTracker> ZoomTriggeredAction;

    public void OnZoomButtonClicked(InputAction.CallbackContext context)
    {
        if (!_isZoomActive && context.performed)
        {
            _isZoomActive = true;
            ZoomTriggeredAction?.Invoke(_isZoomActive, AimTracker);
        }

        else if (_isZoomActive && context.performed)
        {
            _isZoomActive = false;
            ZoomTriggeredAction?.Invoke(_isZoomActive, AimTracker);
        }
    }
}

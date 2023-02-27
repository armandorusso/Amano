using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TetsuoDisableMovement : MonoBehaviour
{
    [SerializeField] public PlayerInput _tetsuoMovement;

    [SerializeField] private PlayerInput _tetsuoAbilities;

    private static TetsuoDisableMovement _instance;
    public static TetsuoDisableMovement Instance
    {
        get { return _instance; }
    }

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        RoomCameraManager.cameraTransitionEvent += OnCameraTransitionEvent;
        TetsuoHealthBar.tetsuoDeathEvent += OnTetsuoDeath;
    }

    private void OnTetsuoDeath(object sender, TetsuoHealthBar.TetsuoDeathEventArgs e)
    {
        if (sender is TetsuoHealthBar)
        {
            DisableInputActions(e.isMovementEnabled);
        }
    }

    private void OnCameraTransitionEvent(object sender, RoomCameraManager.CameraTransitionArgs e)
    {
        if (sender is RoomCameraManager)
        {
            DisableInputActions(e.isMovementDisabled);
        }
    }

    private void DisableInputActions(bool isEnabled)
    {
        _instance._tetsuoMovement.enabled = isEnabled;
        _instance._tetsuoAbilities.enabled = isEnabled;
    }

    private void OnDestroy()
    {
        RoomCameraManager.cameraTransitionEvent -= OnCameraTransitionEvent;
        TetsuoHealthBar.tetsuoDeathEvent -= OnTetsuoDeath;
    }
}

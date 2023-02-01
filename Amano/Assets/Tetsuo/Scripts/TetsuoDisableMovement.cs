using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TetsuoDisableMovement : MonoBehaviour
{
    [SerializeField] public PlayerInput _tetsuoMovement;

    [SerializeField] private PlayerInput _tetsuoAbilities;

    private void Start()
    {
        RoomCameraManager.cameraTransitionEvent += OnCameraTransitionEvent;
    }

    private void OnCameraTransitionEvent(object sender, RoomCameraManager.CameraTransitionArgs e)
    {
        if (sender is RoomCameraManager)
        {
            _tetsuoMovement.enabled = e.isMovementDisabled;
            _tetsuoAbilities.enabled = e.isMovementDisabled;
        }
    }

}

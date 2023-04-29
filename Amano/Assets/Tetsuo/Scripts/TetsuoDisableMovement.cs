using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TetsuoDisableMovement : MonoBehaviour
{
    [SerializeField] public PlayerInput _tetsuoMovement;

    [SerializeField] private PlayerInput _tetsuoAbilities;

    private Rigidbody2D _rb;

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
        TetsuoAnimatorController.EnableInputAction += OnStartGame;
        TetsuoHealthBar.tetsuoDeathEvent += OnTetsuoDeath;

        _rb = GetComponent<Rigidbody2D>();
    }

    public void ResetVelocity()
    {
        _rb.velocity = Vector2.zero;
    }

    private void OnTetsuoDeath(object sender, TetsuoHealthBar.TetsuoDeathEventArgs e)
    {
        if (sender is TetsuoHealthBar)
        {
            EnableOrDisableInputActions(e.isMovementEnabled);
        }
    }

    private void OnCameraTransitionEvent(object sender, RoomCameraManager.CameraTransitionArgs e)
    {
        if (sender is RoomCameraManager)
        {
            // Instead of disabling movement, maybe slow down movement on camera room transition? That's what Celeste does
            if(!e.isMovementDisabled)
                gameObject.transform.position = new Vector2( gameObject.transform.transform.position.x + (gameObject.transform.transform.localScale.x * 0.4f), gameObject.transform.transform.position.y);
            EnableOrDisableInputActions(e.isMovementDisabled);
        }
    }
    
    private void OnStartGame(bool isInputEnabled)
    {
        EnableOrDisableInputActions(isInputEnabled);
    }

    public void EnableOrDisableInputActions(bool isEnabled)
    {
        _instance._tetsuoMovement.enabled = isEnabled;
        _instance._tetsuoAbilities.enabled = isEnabled;
    }

    private void OnDestroy()
    {
        RoomCameraManager.cameraTransitionEvent -= OnCameraTransitionEvent;
        TetsuoHealthBar.tetsuoDeathEvent -= OnTetsuoDeath;
        TetsuoAnimatorController.EnableInputAction -= OnStartGame;
    }
}

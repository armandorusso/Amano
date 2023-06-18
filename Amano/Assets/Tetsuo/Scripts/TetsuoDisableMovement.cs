using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TetsuoDisableMovement : MonoBehaviour
{
    [SerializeField] public PlayerInput _tetsuoMovement;

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
        
        ToggleInputAction("UI");

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
            if(!e.isMovementEnabled)
                gameObject.transform.position = new Vector2( gameObject.transform.transform.position.x + (gameObject.transform.transform.localScale.x * 0.6f), gameObject.transform.transform.position.y);
            EnableOrDisableInputActions(e.isMovementEnabled);
        }
    }
    
    private void OnStartGame(string actionName)
    {
        ToggleInputAction(actionName);
        // EnableOrDisableInputActions(isInputEnabled);
    }

    public void EnableOrDisableInputActions(bool isEnabled)
    {
        _instance._tetsuoMovement.enabled = isEnabled;
    }

    public void DisableMovementForTime(float timeDisabled)
    {
        StartCoroutine(DisableMovementForAFewSeconds(timeDisabled));
    }

    private void ToggleInputAction(string actionName)
    {
        _tetsuoMovement.currentActionMap.Disable();
        _tetsuoMovement.actions.FindActionMap(actionName).Enable();
    }

    private IEnumerator DisableMovementForAFewSeconds(float timeDisabled)
    {
        EnableOrDisableInputActions(false);
        yield return new WaitForSeconds(timeDisabled);
        EnableOrDisableInputActions(true);
    }

    private void OnDestroy()
    {
        RoomCameraManager.cameraTransitionEvent -= OnCameraTransitionEvent;
        TetsuoHealthBar.tetsuoDeathEvent -= OnTetsuoDeath;
        TetsuoAnimatorController.EnableInputAction -= OnStartGame;
    }
}

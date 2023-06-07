using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TetsuoAnimatorController : MonoBehaviour
{
    private Animator _animationController;

    private bool _isJumping;
    private bool _isFalling;
    private bool _isGrounded;
    private bool _isWallSliding;
    private bool _isWallSticking;
    private bool _isDashing;
    private bool _isWalking;
    private bool _isRunning;
    private bool _isSlashing;
    private bool _isSitting;
    private bool _hasDied;

    private TetsuoController _tetsuoController;
    public static Action<string> EnableInputAction;
    public static Action<bool> EnableHealthUiAction;

    private void Awake()
    {
        _animationController = GetComponent<Animator>();
        _isSitting = true;
        _animationController.SetBool("isSitting", true);
    }

    private void Start()
    {
        _tetsuoController = GetComponent<TetsuoController>();
        StartGame.PlayGetUpAnimationAction += OnStartGame;
    }

    private void Update()
    {
        if (_tetsuoController)
        {
            _isJumping = _tetsuoController._isJumping;
            _isFalling = _tetsuoController._isFalling;
            _isGrounded = _tetsuoController._isGrounded;
            _isWallSticking = _tetsuoController._isWallSticking;
            _isWallSliding = _tetsuoController._isWallSliding;
            _isDashing = !_tetsuoController._doneDashing;
            _isWalking = _tetsuoController._isWalking;
            _isRunning = _tetsuoController._isRunning;
            _isSlashing = _tetsuoController._isSlashing;
            _hasDied = GameManager.Instance.isTetsuoDead;
        }

        SetRunningAnimation();
        SetWalkingAnimation();
        SetJumpAnimation();
        SetFallingAnimation();
        SetWallHoldingAnimation();
        SetDashingAnimation();
        SetSlashingAnimation();
        SetDeathAnimation();
    }

    public void SetSittingAnimation()
    {
        _animationController.SetBool("isSitting", true);
    }

    private void SetRunningAnimation()
    {
        _animationController.SetBool("isRunning", _isRunning);
    }

    private void SetWalkingAnimation()
    {
        _animationController.SetBool("isWalking", _isWalking);
    }

    private void SetJumpAnimation()
    {
        _animationController.SetBool("isJumping", _isJumping);
        // Try Crossfade instead? Blends smoothly between animations
    }

    private void SetDashingAnimation()
    {
        _animationController.SetBool("isDashing", _isDashing);
    }

    private void SetFallingAnimation()
    {
        _animationController.SetBool("isFalling", _isFalling);
    }

    private void SetWallHoldingAnimation()
    {
        if (!_isFalling && _isWallSticking)
        {
            _animationController.SetBool("isWallSliding", true);
            return;
        }

        _animationController.SetBool("isWallSliding", _isWallSliding);
        _animationController.SetBool("isWallSticking", _isWallSticking);
    }

    private void SetSlashingAnimation()
    {
        _animationController.SetBool("isSlashing", _isSlashing);
    }
    
    private void OnStartGame(bool hasEnabledInput, float zoomOutTime)
    {
        StartCoroutine(WaitForAnimationToFinish(hasEnabledInput, zoomOutTime));
    }

    public IEnumerator WaitForAnimationToFinish(bool hasEnabledInput, float zoomOutTime)
    {
        yield return new WaitForSeconds(zoomOutTime);
        _isSitting = false;
        _animationController.SetBool("isSitting", false);
        EnableInputAction?.Invoke("Player");
        EnableHealthUiAction?.Invoke(hasEnabledInput);
        StopCoroutine(WaitForAnimationToFinish(hasEnabledInput, zoomOutTime));
    }

    private void SetDeathAnimation()
    {
        _animationController.SetBool("hasDied", _hasDied);
    }
}
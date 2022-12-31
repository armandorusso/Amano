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

    private TetsuoController _tetsuoController;

    private void Start()
    {
        _animationController = GetComponent<Animator>();
        _tetsuoController = GetComponent<TetsuoController>();
    }

    private void Update()
    {
        _isJumping = false;
        _isFalling = false;

        if (_tetsuoController)
        {
            _isJumping = _tetsuoController._isJumping;
            _isFalling = _tetsuoController._isFalling;
            _isGrounded = _tetsuoController._isGrounded;
            _isWallSticking = _tetsuoController._isWallSticking;
            _isWallSliding = _tetsuoController._isWallSliding;
            _isDashing = _tetsuoController._isDashing;
        }

        SetJumpAnimation();
        SetFallingAnimation();
        SetWallHoldingAnimation();
        SetDashingAnimation();
    }

    private void SetDashingAnimation()
    {
        _animationController.SetBool("isDashing", _isDashing);
    }

    private void SetJumpAnimation()
    {
        _animationController.SetBool("isJumping", _isJumping);
        // Try Crossfade instead? Blends smoothly between animations
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
}
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
            _isWallSliding = _tetsuoController._isWallSliding;
        }

        SetJumpAnimation();
        SetFallingAnimation();
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
}

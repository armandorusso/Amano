using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TetsuoController : MonoBehaviour
{
    [SerializeField] public Rigidbody2D rb;
    [SerializeField] public Transform groundCheck;
    [SerializeField] public LayerMask groundLayer;
    [SerializeField] public Animator animator;

    private float _horizontal;
    private float _speed = 4f;
    private float _jumpingPower = 12f;
    private bool _isFacingRight = true;
    private float _isFacingRightScale = 1f;
    private SpriteRenderer _sprite;

    [SerializeField] private float wallSlideSpeed = 0f;
    [SerializeField] public Transform wallCheckPoint;
    [SerializeField] public LayerMask wallLayer;
    private bool isWallSliding;

    [SerializeField] private Vector2 wallJumpPower;
    private float wallJumpDirection;
    private bool _isWallJumping = false;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.4f;

    void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
        wallJumpDirection = -1f;
    }

    void Update()
    {
        if(!_isWallJumping)
            rb.velocity = new Vector2(_horizontal * _speed, rb.velocity.y);
        WallSlide();
        CheckIfPlayerCanWallJump();
        switch (_isFacingRight)
        {
            case false when _horizontal > 0f:
            case true when _horizontal < 0f:
                if(!_isWallJumping)
                    Flip();
                break;
        }
        SetAnimatorState();
    }

    private void FixedUpdate()
    {
        SetAnimatorState();
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private bool IsTouchingWall()
    {
        return Physics2D.OverlapCircle(wallCheckPoint.position, 0.2f, wallLayer);
    }

    private void WallSlide()
    {
        if (IsTouchingWall() && !IsGrounded() &&
            _horizontal != 0) // if you started falling down while touching the wall
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Math.Clamp(rb.velocity.y, -wallSlideSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
        
        _sprite.flipX = isWallSliding;
    }

    public void WallJump(InputAction.CallbackContext context)
    {
        if (context.performed && wallJumpingCounter > 0f)
        {
            _isWallJumping = true;
            rb.velocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y);
            wallJumpingCounter = 0f;

            if (transform.localScale.x != wallJumpDirection)
            {
                Flip();
            }
            
            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    private void CheckIfPlayerCanWallJump()
    {
        if (isWallSliding)
        {
            _isWallJumping = false;
            wallJumpDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;
            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }
    }

    private void StopWallJumping()
    {
        _isWallJumping = false;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, _jumpingPower);
        }

        if (context.canceled && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
    }

    private void Flip()
    {
        _isFacingRight = !_isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    public void Move(InputAction.CallbackContext context)
    {
        _horizontal = context.ReadValue<Vector2>().x;
    }

    public void Run(InputAction.CallbackContext context)
    {
        if(_horizontal is > 0f or < 0f && context.performed)
            _speed = 6f;
        else if (context.canceled)
            _speed = 4f;
    }

    public void SetAnimatorState()
    {
        if (_horizontal is > 0f or < 0f && IsGrounded())
        {
            if (_speed == 4f)
            {
                animator.SetBool("isWalking", true);
                animator.SetBool("isRunning", false);
            }
            else if (_speed == 6f)
            {
                animator.SetBool("isWalking", false);
                animator.SetBool("isRunning", true);
            }
        }
        else
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
        }

        if (isWallSliding)
        {
            animator.SetBool("isWallSliding", true);
        }
        else
        {
            animator.SetBool("isWallSliding", false);
        }
    }
}

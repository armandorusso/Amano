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
    private bool _isJumping;
    private bool _isFacingRight = true;
    private float _isFacingRightScale = 1f;
    public bool isGrounded;
    private SpriteRenderer _sprite;

    [SerializeField] private float wallSlideSpeed = 0f;
    [SerializeField] public Transform wallCheckPoint;
    [SerializeField] public LayerMask wallLayer;
    public bool isWallSliding;

    [SerializeField] private Vector2 wallJumpPower;
    private float wallJumpDirection;
    public bool _isWallJumping;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.4f;

    public class WallSlidingFxEventArgs : EventArgs
    {
        public bool isSliding { get; set; }
        public bool isFacingRight { get; set; }
    }

    public static event EventHandler<WallSlidingFxEventArgs> wallSlidingEvent;
    private WallSlidingFxEventArgs wallSlidingEventArgs;
    
    public class GroundFxEventArgs : EventArgs
    {
        public bool isDustActivated { get; set; }
    }
    
    public static event EventHandler<GroundFxEventArgs> jumpOrLandEvent;
    private GroundFxEventArgs jumpOrLandEventArgs;

    void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
        wallJumpDirection = -1f;
        wallSlidingEventArgs = new WallSlidingFxEventArgs
        {
            isSliding = isWallSliding,
            isFacingRight = _isFacingRight
        };

        jumpOrLandEventArgs = new GroundFxEventArgs
        {
            isDustActivated = false
        };
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
        // var ray = Physics2D.Raycast(groundCheck.position, Vector2.down);
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        return isGrounded;
    }

    private bool IsTouchingWall()
    {
        return Physics2D.OverlapCircle(wallCheckPoint.position, 0.2f, wallLayer);
    }

    private void WallSlide()
    {
        if (IsTouchingWall() && !IsGrounded() &&
            _horizontal != 0) // if you are falling and are running towards the wall
        {
            isWallSliding = true;
            wallSlidingEventArgs.isSliding = isWallSliding;
            wallSlidingEventArgs.isFacingRight = _isFacingRight;
            wallSlidingEvent.Invoke(this, wallSlidingEventArgs);
            rb.velocity = new Vector2(rb.velocity.x, Math.Clamp(rb.velocity.y, -wallSlideSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
            wallSlidingEventArgs.isSliding = false;
            wallSlidingEventArgs.isFacingRight = _isFacingRight;
            wallSlidingEvent.Invoke(this, wallSlidingEventArgs);
        }
        
        _sprite.flipX = isWallSliding;
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

    private void StopWallJumping()
    {
        _isWallJumping = false;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && IsGrounded())
        {
            animator.SetBool("isJumping", true);
            jumpOrLandEventArgs.isDustActivated = true;
            jumpOrLandEvent.Invoke(this, jumpOrLandEventArgs);
            rb.velocity = new Vector2(rb.velocity.x, _jumpingPower);
        }

        if (context.canceled && rb.velocity.y > 0)
        {
            animator.SetBool("isJumping", false);
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
        jumpOrLandEventArgs.isDustActivated = false;
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
        animator.SetBool("isJumping", false);
        _horizontal = context.ReadValue<Vector2>().x;
    }

    public void Run(InputAction.CallbackContext context)
    {
        if (_horizontal is > 0f or < 0f && context.performed)
        {
            _speed = 6f;
        }
        else if (context.canceled)
        {
            _speed = 4f;
        }
    }

    public void SetAnimatorState()
    {
        if (_horizontal is > 0f or < 0f && IsGrounded())
        {
            if (_speed == 4f)
            {
                animator.SetBool("isJumping", false);
                animator.SetBool("isWalking", true);
                animator.SetBool("isRunning", false);
            }
            else if (_speed == 6f)
            {
                animator.SetBool("isJumping", false);
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
            animator.SetBool("isJumping", false);
            animator.SetBool("isWallSliding", true);
        }
        else
        {
            animator.SetBool("isWallSliding", false);
        }
    }

}

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
    public bool _isJumping { get; private set; }
    private bool _isFacingRight = true;
    private float _isFacingRightScale = 1f;
    public bool _isGrounded { get; set; }
    private SpriteRenderer _sprite;

    [SerializeField] private float wallSlideSpeed = 0f;
    [SerializeField] public Transform wallCheckPoint;
    [SerializeField] public LayerMask wallLayer;
    public bool _isWallSliding { get; private set; }

    [SerializeField] private Vector2 wallJumpPower;
    private float wallJumpDirection;
    public bool _isWallJumping { get; private set; }
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.4f;

    public bool _isFalling { get; private set; }

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
    
    public class AnimationEventArgs : EventArgs
    {
        public bool isAnimationActivated { get; set; }
    }
    public static event EventHandler<AnimationEventArgs> animationEvent;
    private AnimationEventArgs fallingEventArgs;
    
    

    void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
        wallJumpDirection = -1f;
        wallSlidingEventArgs = new WallSlidingFxEventArgs
        {
            isSliding = _isWallSliding,
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

        UpdateIsFalling();
    }

    private void FixedUpdate()
    {
        SetAnimatorState();
    }

    private bool IsGrounded()
    {
        _isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        
        if (_isGrounded)
            _isFalling = false;
        
        return _isGrounded;
    }

    private bool IsTouchingWall()
    {
        return Physics2D.OverlapCircle(wallCheckPoint.position, 0.2f, wallLayer);
    }

    private bool UpdateIsFalling()
    {
        if (!_isWallSliding && !_isGrounded && rb.velocity.y < 0)
        {
            _isFalling = true;
        }

        return _isFalling;
    }

    private void WallSlide()
    {
        if (IsTouchingWall() && !IsGrounded() &&
            _horizontal != 0) // if you are falling and are running towards the wall
        {
            _isWallSliding = true;
            _isFalling = false;
            wallSlidingEventArgs.isSliding = _isWallSliding;
            wallSlidingEventArgs.isFacingRight = _isFacingRight;
            wallSlidingEvent.Invoke(this, wallSlidingEventArgs);
            rb.velocity = new Vector2(rb.velocity.x, Math.Clamp(rb.velocity.y, -wallSlideSpeed, float.MaxValue));
        }
        else
        {
            _isWallSliding = false;
            wallSlidingEventArgs.isSliding = false;
            wallSlidingEventArgs.isFacingRight = _isFacingRight;
            wallSlidingEvent.Invoke(this, wallSlidingEventArgs);
        }
        
        _sprite.flipX = _isWallSliding;
    }

    private void CheckIfPlayerCanWallJump()
    {
        if (_isWallSliding)
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
            _isFalling = false;
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
            _isJumping = true;
            jumpOrLandEventArgs.isDustActivated = true;
            jumpOrLandEvent.Invoke(this, jumpOrLandEventArgs);
            rb.velocity = new Vector2(rb.velocity.x, _jumpingPower);
        }

        if (context.canceled && rb.velocity.y > 0)
        {
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
            switch (_speed)
            {
                case 4f:
                    animator.SetBool("isJumping", false);
                    animator.SetBool("isWalking", true);
                    animator.SetBool("isRunning", false);
                    break;
                case 6f:
                    animator.SetBool("isJumping", false);
                    animator.SetBool("isWalking", false);
                    animator.SetBool("isRunning", true);
                    break;
            }
        }
        else
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
        }

        if (_isWallSliding)
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

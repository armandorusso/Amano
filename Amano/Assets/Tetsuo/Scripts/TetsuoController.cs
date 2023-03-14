using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TetsuoController : MonoBehaviour, IMove
{
    [SerializeField] public Rigidbody2D rb;
    [SerializeField] public Transform groundCheck;
    [SerializeField] public LayerMask groundLayer;
    [SerializeField] public Animator animator;
    private PlayerInput _inputAction;

    [Header("Movement")] 
    [SerializeField] public float MaxRunSpeed;
    [SerializeField] public float MaxAccelSpeed;
    [SerializeField] public float MaxAirAccelSpeed;
    [SerializeField] public float MaxDeAccelSpeed;
    [SerializeField] public float MaxDeAccelAirSpeed;
    [SerializeField] public float RunLerpAmount;
    [SerializeField] public bool ConserveMomentum;
    [SerializeField] public float MaxFallSpeed;
    [SerializeField] public float FallGravityMultiplier;
    private float _speed = 6f;

    private float _horizontal;
    private float _vertical;
    public bool _isGrounded { get; set; }
    public bool _isWalking { get; private set; }
    public bool _isRunning { get; private set; }
    public bool _hasLanded { get; private set; }
    private bool _isFacingRight { get; set; }
    private float _isFacingRightScale = 1f;
    private SpriteRenderer _sprite;
    private Color _spriteOriginalColor;

    [Header("Jumping")] 
    public float JumpHangTime;
    public float _jumpingPower = 12f;
    public float coyoteTime = 0.2f;
    public float coyoteTimeCounter;
    public float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;
    public bool _isJumping { get; private set; }
    
    public bool _isJumpFalling { get; private set; }

    public bool _isFalling { get; private set; }

    public class GroundFxEventArgs : EventArgs
    {
        public bool isDustActivated { get; set; }
    }
    public static event EventHandler<GroundFxEventArgs> jumpOrLandEvent;
    private GroundFxEventArgs jumpOrLandEventArgs;

    [Header("Wall Sliding")]
    [SerializeField] private float wallSlideSpeed = 0f;
    [SerializeField] public Transform wallCheckPoint;
    [SerializeField] public LayerMask wallLayer;
    public bool _isWallSliding { get; private set; }
    public class WallSlidingFxEventArgs : EventArgs
    {
        public bool isSliding { get; set; }
        public bool isFacingRight { get; set; }
    }
    public static event EventHandler<WallSlidingFxEventArgs> wallSlidingEvent;
    private WallSlidingFxEventArgs wallSlidingEventArgs;

    [Header("Wall Jumping")]
    [SerializeField] private Vector2 wallJumpPower;
    private float wallJumpDirection;
    public bool _isWallJumping { get; private set; }
    public float wallJumpingTime;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.4f;

    [Header("Wall Sticking")]
    public float _WallStickingTimer = 2f;

    public bool _isWallSticking { get; private set; }
    private Color _outOfStaminaColor = Color.red;
    private float _originalGravityScale;
    
    [Header("Dash Attack")]
    [SerializeField] public float dashPower;
    [SerializeField] public float dashTime;
    [SerializeField] public float dashCooldown;
    [SerializeField] public float dashCooldownOnGround;
    private bool _isPerformingGroundedDash { get; set; }
    public class DashAttackFxEventArgs : EventArgs
    {
        public bool isDashing { get; set; }
    }
    public static event EventHandler<DashAttackFxEventArgs> dashAttackEvent;
    private DashAttackFxEventArgs dashAttackEventArgs;
    public class GroundedDashAttackFxEventArgs : EventArgs
    {
        public GameObject Tetsuo { get; set; }
    }
    public static event EventHandler<GroundedDashAttackFxEventArgs> groundedDashAttackEvent;
    private GroundedDashAttackFxEventArgs groundedDashAttackEventArgs;
    public bool _hasDashed { get; private set; }
    public bool _isAirDashing { get; private set; }
    public bool _isGroundDashing { get; private set; }
    public bool _doneDashing { get; private set; }

    void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _inputAction = GetComponent<PlayerInput>();
        _spriteOriginalColor = _sprite.color;
        wallJumpDirection = -1f;
        _originalGravityScale = rb.gravityScale;
        wallSlidingEventArgs = new WallSlidingFxEventArgs
        {
            isSliding = _isWallSliding,
            isFacingRight = _isFacingRight
        };

        jumpOrLandEventArgs = new GroundFxEventArgs
        {
            isDustActivated = false
        };

        dashAttackEventArgs = new DashAttackFxEventArgs
        {
            isDashing = false
        };

        groundedDashAttackEventArgs = new GroundedDashAttackFxEventArgs
        {
            Tetsuo = gameObject
        };
        
        _isWalking = false;
        _isRunning = true;
        _isFacingRight = true;
        _doneDashing = true;
    }

    void Update()
    {
        switch (_isFacingRight)
        {
            case false when _horizontal > 0f:
            case true when _horizontal < 0f:
                if(!_isWallJumping)
                    Flip();
                break;
        }
        UpdateIsFalling();
        UpdateIsRunning();
        UpdateJumpFalling();
        ModifyJumpPeakGravity();
    }

    private void FixedUpdate()
    {
        if (_isAirDashing || _isGroundDashing)
            return;
        UpdateIsGrounded();
        StickToWall();
        WallSlide();
        CheckIfPlayerCanWallJump();

        if (!_isWallJumping)
            Move();
    }

    private void UpdateJumpFalling()
    {
        if (_isJumping && !_isWallJumping && rb.velocity.y < 0f)
        {
            _isJumping = false;
            _isJumpFalling = true;
        }
    }

    private void ModifyJumpPeakGravity()
    {
        if ((_isJumping || _isWallJumping || _isJumpFalling) && Mathf.Abs(rb.velocity.y) < 0.4f)
        {
            SetGravityScale(rb.gravityScale * 0.5f);
        }
        else if(!_hasDashed)
        {
            SetGravityScale(_originalGravityScale);
        }
    }

    private void SetGravityScale(float newGravityScale)
    {
        rb.gravityScale = newGravityScale;
    }

    private void UpdateIsGrounded()
    {
        _isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
        if (_isGrounded)
        {
            _isWallSticking = false;
            _isJumpFalling = false;
            _WallStickingTimer = 2f;
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            _hasLanded = false;
            coyoteTimeCounter-= Time.deltaTime;
        }

        Debug.Log("Is Ground: " + _isGrounded);
    }

    private void UpdateIsFalling()
    {
        if (!_isWallSliding && !_isWallSticking && !_isGrounded && rb.velocity.y < 0)
        {
            _isFalling = true;
            ClampGravity();
        }
        else
        {
            _isFalling = false;
        }
    }
    
    private void UpdateIsRunning()
    {
        if (_horizontal is > 0f or < 0f && _isGrounded)
        {
            _isRunning = true;
        }
        else
        {
            _isRunning = false;
        }
    }

    private void ClampGravity()
    {
        SetGravityScale(_originalGravityScale * FallGravityMultiplier);
        rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -MaxFallSpeed));
    }

    private void UpdateHasLanded(Collision2D collision)
    {
        if (_hasLanded)
            return;
        
        if (collision.gameObject.CompareTag("Ground"))
        {
            Debug.Log("Landed");
            _hasLanded = true;
            _hasDashed = false;
            
            jumpOrLandEventArgs.isDustActivated = true;
            jumpOrLandEvent.Invoke(this, jumpOrLandEventArgs);
            jumpOrLandEventArgs.isDustActivated = false;
        }
    }

    private bool IsTouchingWall()
    {
        return Physics2D.OverlapBox(wallCheckPoint.position, new Vector2(0.03956366f, 0.7018313f), 0, wallLayer, 0, 0);
    }

    private void StickToWall()
    {
        if (IsTouchingWall() && !_isGrounded && _horizontal != 0 && _WallStickingTimer > 0f)
        {
            _isWallSticking = true;
            _isWallSliding = false;
            rb.gravityScale = 0f;
            rb.velocity = Vector2.zero;
            _sprite.color = Color.Lerp(_sprite.color, _outOfStaminaColor, Time.deltaTime / _WallStickingTimer);
            _WallStickingTimer -= Time.deltaTime;
        }
        else
        {
            _isWallSticking = false;
            rb.gravityScale = _originalGravityScale;
            if(_isGrounded)
                _sprite.color = _spriteOriginalColor;
        }
        
        _sprite.flipX = _isWallSticking;
    }

    private void WallSlide()
    {
        if (IsTouchingWall() && !_isGrounded && !_isWallSticking) // if you are falling and are running towards the wall
        {
            _sprite.flipX = _isWallSliding;
            rb.gravityScale = _originalGravityScale;
            _isWallSliding = true;
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
    }

    private void CheckIfPlayerCanWallJump()
    {
        if (_isWallSliding || _isWallSticking)
        {
            // _isWallJumping = false;
            wallJumpDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }
    }

    public void WallJump(InputAction.CallbackContext context)
    {
        Debug.Log("WallJumped: " + IsTouchingWall());
        if (context.performed && IsTouchingWall() && (_isWallSticking || _isWallSliding) && wallJumpingCounter > 0f)
        {
            Debug.Log("Walljump direction: " + wallJumpDirection);
            _isWallJumping = true;
            rb.gravityScale = _originalGravityScale;
            var wallJumpVector = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y);
            rb.AddForce(wallJumpVector, ForceMode2D.Impulse);
            wallJumpingCounter = 0f;

            if (transform.localScale.x != wallJumpDirection)
            {
                Flip();
            }
            
            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (_isWallJumping || _isWallSliding || _isWallSticking)
            return;
        
        if (context.started || context.performed)
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else if(!_isJumping)
        {
            jumpBufferCounter -= Time.deltaTime;
        }
        
        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f && !(_isWallSliding || _isWallSticking))
        {
            jumpOrLandEventArgs.isDustActivated = true;
            jumpOrLandEvent.Invoke(this, jumpOrLandEventArgs);
            _isJumping = true;
            rb.velocity = new Vector2(rb.velocity.x, _jumpingPower);
            jumpBufferCounter = 0f;
        }

        if (context.canceled && rb.velocity.y > 0 && !(_isWallSliding || _isWallSticking))
        {
            _isJumping = true;
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            coyoteTimeCounter = 0f;
        }

        jumpOrLandEventArgs.isDustActivated = false;
    }

    private void StopWallJumping()
    {
        _isWallJumping = false;
        CancelInvoke(nameof(StopWallJumping));
    }

    private void Flip()
    {
        _isFacingRight = !_isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _horizontal = context.ReadValue<Vector2>().x;
        _vertical = context.ReadValue<Vector2>().y;
    }

    public void Move()
    {
        // Force based movement with accel and deaccel, courtesy of https://github.com/Dawnosaur/platformer-movement/blob/main/Scripts/PlayerMovement.cs#L255
        var targetSpeed = _horizontal * MaxRunSpeed;
        
        // Makes reaching max speed a lot smoother (and not instant)
        targetSpeed = Mathf.Lerp(rb.velocity.x, targetSpeed, 1f);

        float acceleration;

        if (_isGrounded)
        {
            acceleration = Mathf.Abs(targetSpeed) > 0.04 ? MaxAccelSpeed : MaxDeAccelSpeed;
        }
        else
        {
            acceleration = Mathf.Abs(targetSpeed) > 0.04 ? MaxAccelSpeed * MaxAirAccelSpeed : MaxDeAccelSpeed * MaxDeAccelAirSpeed;
        }
        
        // Conserving Momentum
        if (ConserveMomentum && Mathf.Abs(rb.velocity.x) > Mathf.Abs(targetSpeed) &&
            Mathf.Sign(rb.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Sign(targetSpeed) > 0.04f && !_isGrounded)
        {
            acceleration = 0f;
        }
        
        // Calculate difference between current velocity and desired velocity
        float speedDif = targetSpeed - rb.velocity.x;
        
        // Calculate force along x-axis to apply to thr player
        float movement = speedDif * acceleration;

        // Convert this to a vector and apply to rigidbody
        rb.AddForce(movement * Vector2.right, ForceMode2D.Force);
    }

    public Vector2 GetMovementVector()
    {
        return new Vector2(_horizontal, _vertical);
    }

    public void DashAttack(InputAction.CallbackContext context)
    {
        if (context.started && !_hasDashed)
        {
            if (!_isGrounded)
                StartCoroutine(Dash());
            else
                StartCoroutine(GroundedDashAttack());
        }
    }

    private IEnumerator Dash()
    {
        Debug.Log("Dash");
        var dashCoolDown = dashCooldown;
        SetDashParameters();
        SetGravityScale(0f);
        
        rb.velocity = _horizontal != 0 || _vertical != 0
                ? new Vector2( _horizontal, _vertical).normalized * dashPower
                : new Vector2(dashPower * transform.localScale.x, 0f);

        _isAirDashing = true;
        yield return null;
        yield return new WaitForSeconds(dashTime);
        dashAttackEventArgs.isDashing = false;
        dashAttackEvent.Invoke(this, dashAttackEventArgs);
        SetGravityScale(_originalGravityScale);
        _isAirDashing = false;
        yield return new WaitForSeconds(dashCoolDown);
        _doneDashing = true;
        Debug.Log("Has landed: "+ _hasLanded + " Is Grounded " + _isGrounded);
        if (_isGrounded)
        {
            Debug.Log("Resetting dash");
            _sprite.color = _spriteOriginalColor;
            _hasDashed = false;
        }
    }

    private IEnumerator GroundedDashAttack()
    {
        Debug.Log("Grounded Dash");
        var dashCoolDown = dashCooldownOnGround;
        SetDashParameters();

        _isPerformingGroundedDash = true;
        _inputAction.enabled = false;
        yield return new WaitForSeconds(dashTime);
        rb.velocity = new Vector2(dashPower * transform.localScale.x, 0f);
        groundedDashAttackEvent.Invoke(this, groundedDashAttackEventArgs);
        _isGroundDashing = true;
        yield return new WaitForSeconds(dashTime);
        _inputAction.enabled = true;
        _isPerformingGroundedDash = false;
        dashAttackEventArgs.isDashing = false;
        dashAttackEvent.Invoke(this, dashAttackEventArgs);
        SetGravityScale(_originalGravityScale);
        _isGroundDashing = false;
        yield return new WaitForSeconds(dashCoolDown);
        _doneDashing = true;
        _hasDashed = false;
    }

    private void SetDashParameters()
    {
        _doneDashing = false;
        _hasDashed = true;
        dashAttackEventArgs.isDashing = true;
        dashAttackEvent.Invoke(this, dashAttackEventArgs);
        _sprite.color = new Color(Color.cyan.r, Color.cyan.g, Color.cyan.b, 150); // Temp color
    }

    public void Walk(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        
        if (_isRunning)
        {
            _isRunning = false;
            _isWalking = true;
            _speed = 4f;
        }
        else
        {
            _isRunning = true;
            _isWalking = false;
            _speed = 6f;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        // Gizmos.DrawCube(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y), new Vector2(1, 1));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        UpdateHasLanded(collision);
    }
}

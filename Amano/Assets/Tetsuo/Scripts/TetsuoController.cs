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
    
    [Header("Movement")]
    private float _horizontal;
    private float _vertical;
    private float _speed = 6f;
    public bool _isGrounded { get; set; }
    public bool _isWalking { get; private set; }
    public bool _isRunning { get; private set; }
    public bool _hasLanded { get; private set; }
    private bool _isFacingRight { get; set; }
    private float _isFacingRightScale = 1f;
    private SpriteRenderer _sprite;
    private Color _spriteOriginalColor;
        
    public class GroundFxEventArgs : EventArgs
    {
        public bool isDustActivated { get; set; }
    }
    public static event EventHandler<GroundFxEventArgs> jumpOrLandEvent;
    private GroundFxEventArgs jumpOrLandEventArgs;

    [Header("Jumping")]
    private float _jumpingPower = 12f;
    public bool _isJumping { get; private set; }
    public bool _isFalling { get; private set; }

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
    private float wallJumpingTime;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.4f;

    [Header("Wall Sticking")]
    private Color _outOfStaminaColor = Color.red;
    public bool _isWallSticking { get; private set; }
    private float _gravityScale;
    private float _WallStickingTimer = 2f;

    [Header("Dash Attack")]
    [SerializeField] public float dashPower;
    [SerializeField] public float dashTime;
    [SerializeField] public float dashCooldown;
    [SerializeField] public float dashCooldownOnGround;
    private TrailRenderer _dashTrail;
    public bool _hasDashed { get; private set; }
    public bool _isDashing { get; private set; }
    public bool _doneDashing { get; private set; }

    void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _dashTrail = GetComponent<TrailRenderer>();
        _spriteOriginalColor = _sprite.color;
        wallJumpDirection = -1f;
        wallJumpingTime = 0.2f;
        _gravityScale = rb.gravityScale;
        wallSlidingEventArgs = new WallSlidingFxEventArgs
        {
            isSliding = _isWallSliding,
            isFacingRight = _isFacingRight
        };

        jumpOrLandEventArgs = new GroundFxEventArgs
        {
            isDustActivated = false
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
    }

    private void FixedUpdate()
    {
        if (_isDashing)
            return;
        UpdateIsGrounded();
        UpdateIsFalling();
        StickToWall();
        WallSlide();
        CheckIfPlayerCanWallJump();
        
        if(!_isWallJumping)
            rb.velocity = new Vector2(_horizontal * _speed, rb.velocity.y);
        
        SetAnimatorState();
    }

    private bool UpdateIsGrounded()
    {
        _isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
        if (_isGrounded)
        {
            _isWallSticking = false;
            _WallStickingTimer = 2f;
        }
        else
        {
            _hasLanded = false;
        }
        Debug.Log("Is Ground: " + _isGrounded);
        return _isGrounded;
    }

    private void UpdateIsFalling()
    {
        if ( (!_isWallSliding || !_isWallSticking) && !_isGrounded && rb.velocity.y < 0)
        {
            _isFalling = true;
            _isJumping = false;
        }
        else
        {
            _isFalling = false;
        }
    }

    private void UpdateHasLanded(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Debug.Log("Landed");
            _hasLanded = true;
            _hasDashed = false;
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
            rb.gravityScale = _gravityScale;
            if(_isGrounded)
                _sprite.color = _spriteOriginalColor;
        }
        
        _sprite.flipX = _isWallSticking;
    }

    private void WallSlide()
    {
        if (IsTouchingWall() && !_isGrounded && !_isWallSticking
            && _horizontal != 0) // if you are falling and are running towards the wall
        {
            _sprite.flipX = _isWallSliding;
            rb.gravityScale = _gravityScale;
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
        Debug.Log("WallJumped: " + IsTouchingWall());
        if (context.performed && (_isWallSticking || _isWallSliding) && wallJumpingCounter > 0f)
        {
            Debug.Log("Walljump direction: " + wallJumpDirection);
            _isWallJumping = true;
            rb.gravityScale = _gravityScale;
            rb.velocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y);
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
        if ((context.started || context.performed) && _isGrounded && !(_isWallSliding || _isWallSticking))
        {
            jumpOrLandEventArgs.isDustActivated = true;
            jumpOrLandEvent.Invoke(this, jumpOrLandEventArgs);
            _isJumping = true;
            rb.velocity = new Vector2(rb.velocity.x, _jumpingPower);
        }

        if (context.canceled && rb.velocity.y > 0)
        {
            _isJumping = true;
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }

        jumpOrLandEventArgs.isDustActivated = false;
    }

    private void StopWallJumping()
    {
        _isWallJumping = false;
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
        _vertical = context.ReadValue<Vector2>().y;
    }

    public void DashAttack(InputAction.CallbackContext context)
    {
        if (context.started && !_hasDashed)
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        Debug.Log("Dash");
        _doneDashing = false;
        _hasDashed = true;
        rb.gravityScale = 0f;
        _dashTrail.emitting = true;
        _sprite.color = new Color(Color.cyan.r, Color.cyan.g, Color.cyan.b, 150); // Temp color
        rb.velocity = new Vector2(dashPower * _horizontal, dashPower * _vertical);
        _isDashing = true;
        yield return new WaitForSeconds(dashTime);
        _dashTrail.emitting = false;
        rb.gravityScale = _gravityScale;
        _isDashing = false;
        yield return new WaitForSeconds(_isGrounded? dashCooldownOnGround : dashCooldown);
        _doneDashing = true;
        Debug.Log("Has landed: "+ _hasLanded + " Is Grounded " + _isGrounded);
        if (_hasLanded || _isGrounded)
        {
            Debug.Log("Resetting dash");
            _sprite.color = _spriteOriginalColor;
        }
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

    private void SetAnimatorState()
    {
        if (_horizontal is > 0f or < 0f && _isGrounded)
        {
            animator.SetBool("isWalking", _isWalking);
            animator.SetBool("isRunning", _isRunning);
        }
        else
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
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

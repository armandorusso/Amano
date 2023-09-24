using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class TetsuoController : MonoBehaviour, IMove
{
    [SerializeField] public Rigidbody2D rb;
    [SerializeField] public Transform groundCheck;
    [SerializeField] public LayerMask groundLayer;
    [SerializeField] public Transform wallCheckPoint;
    [SerializeField] public TetsuoScriptableObject TetsuoData;
    [SerializeField] public InputAction JumpBoostButton;
    [SerializeField] public FactsScriptableObject AbilityFacts;

    private PlayerInput _inputAction;
    private LayerMask _collidedLayer;
    private float _horizontal;
    private float _vertical;
    private bool _isFacingRight { get; set; }
    private float _isFacingRightScale = 1f;
    private float _originalGravityScale;
    
    public bool _isGrounded { get; set; }
    public bool _isWalking { get; private set; }
    public bool _isRunning { get; private set; }
    public bool _hasLanded { get; private set; }
    
    public bool hasPressedJump { get; set; }
    public bool hasReleasedJump { get; set; }
    public bool _isJumping { get; private set; }
    public bool _isJumpFalling { get; private set; }
    
    public bool _isFalling { get; private set; }
    private float jumpBufferCounter;
    private bool _isShortHop { get; set; }
    private bool _isFullJump { get; set; }
    public bool hasActivatedTeleportPopOut { get; set; }
    private float teleportPopOutTimer = 0.08f;
    private SpriteRenderer _sprite;
    private Color _spriteOriginalColor;
    private Color _outOfStaminaColor = Color.red;
    
    public bool _isWallSticking { get; private set; }
    public bool _isWallSliding { get; private set; }
    
    private float wallJumpFacingDirection;
    public bool _isWallJumping { get; private set; }
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.4f;
    
    private bool _isPerformingGroundedDash { get; set; }
    public bool _hasDashed { get; private set; }
    public bool _isAirDashing { get; private set; }
    public bool _isGroundDashing { get; private set; }
    public bool _doneDashing { get; private set; }
    
    public bool _isSlashing { get; private set; }


    public static Action<bool> JumpOrLandEffectAction;
    public static Action<bool, bool> WallSlidingEffectAction;
    public static Action<bool> DashAttackLeafEffectAction;
    public static Action<bool> MaxSpeedExceedEffectAction;
    public static Action<string> PlaySoundEffectAction;
    public static Action<string> PlayExtraSoundEffectAction;
    public static Action<bool> PlayRunSoundEffectAction;
    public static Action<string> StopSoundEffectAction;

    public class GroundedDashAttackFxEventArgs : EventArgs
    {
        public GameObject Tetsuo { get; set; }
    }
    public static event EventHandler<GroundedDashAttackFxEventArgs> groundedDashAttackEvent;
    private GroundedDashAttackFxEventArgs groundedDashAttackEventArgs;
    
    void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _inputAction = GetComponent<PlayerInput>();
        JumpBoostButton.Enable();
        _spriteOriginalColor = _sprite.color;
        wallJumpFacingDirection = -1f;
        _originalGravityScale = rb.gravityScale;

        groundedDashAttackEventArgs = new GroundedDashAttackFxEventArgs
        {
            Tetsuo = gameObject
        };
        
        _isWalking = false;
        _isRunning = true;
        _isFacingRight = true;
        _doneDashing = true;

        FastMovingPlatform.TouchingPlatformAction += OnLeavingPlatform;
        TeleportAbility.TeleportPopOutAction += OnTeleportPopOut;
        TetsuoSlashAttack.SlashBoostAction += OnSlashBoost;
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
        // CheckIfPlayerCanWallJump();
        Jump();
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
        // StickToWall();
        WallSlide();

        if (!_isWallJumping)
            Move();
    }
    
    public void OnMove(InputAction.CallbackContext context)
    {
        var movementVector = new Vector2(context.ReadValue<Vector2>().x, context.ReadValue<Vector2>().y).normalized;
        _horizontal = Mathf.Round(movementVector.x);
        _vertical = Mathf.Round(movementVector.y);
    }
    
    public void OnDashAttack(InputAction.CallbackContext context)
    {
        if (AbilityFacts.Facts["Dash"] == 0)
        {
            return;
        }
        
        if (context.started && !_hasDashed)
        {
            StartCoroutine(Dash());
        }
    }
    
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            hasPressedJump = true;
            Invoke(nameof(SetTeleportPopOutBooleanToFalse), teleportPopOutTimer);
        }
        else if (context.canceled)
        {
            // Currently not used right now
            hasReleasedJump = true;
        }
    }

    public void WallJump(InputAction.CallbackContext context)
    {
        Debug.Log("WallJumped: " + IsTouchingWall());
        if (context.performed && !_isGrounded && IsTouchingWall() && (!_hasDashed || !_isGrounded || _isWallSticking || _isWallSliding))
        {
            CancelInvoke(nameof(StopWallJumping));
            wallJumpFacingDirection = -transform.localScale.x;
            Debug.Log("Walljump direction: " + wallJumpFacingDirection);
            _isWallJumping = true;
            hasPressedJump = false;
            SetGravityScale(_originalGravityScale);
            var wallJumpVector = new Vector2(wallJumpFacingDirection * TetsuoData.wallJumpingDirection.x * TetsuoData.wallJumpForce, TetsuoData.wallJumpingDirection.y * TetsuoData.wallJumpForce);
            rb.velocity = wallJumpVector;
            PlaySoundEffectAction?.Invoke("Jump");
            
            // wallJumpingCounter = 0.28f;

            if (transform.localScale.x != wallJumpFacingDirection)
            {
                Flip();
            }
            
            Invoke(nameof(StopWallJumping), TetsuoData.wallJumpingTime);
        }
    }
    
    public void Slash(InputAction.CallbackContext context)
    {
        if (context.performed)
            return;
        
        if(context.started)
        {
            PlaySoundEffectAction?.Invoke("Slash");
            _isSlashing = true;
            Invoke(nameof(StopSlashing), TetsuoData.SlashingTime);
        }
    }
    
    public void Walk(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        
        if (_isRunning)
        {
            _isRunning = false;
            _isWalking = true;
            TetsuoData._speed = 4f;
        }
        else
        {
            _isRunning = true;
            _isWalking = false;
            TetsuoData._speed = 6f;
        }
    }

    public void StopSlashing()
    {
        _isSlashing = false;
        CancelInvoke(nameof(StopSlashing));
    }
    
    private void OnLeavingPlatform(Vector2 platformVelocity)
    {
        // While running off + jumping a fast moving platform, gain a boost of speed
        if (JumpBoostButton.IsPressed() && _isJumping && _isRunning)
        {
            rb.velocity += platformVelocity;
            MaxSpeedExceedEffectAction?.Invoke(true);
        }
    }
    
    private void OnTeleportPopOut(float popOutForce)
    {
        if (JumpBoostButton.IsPressed() && (_isGrounded || _isJumping || _isFalling))
        {
            hasActivatedTeleportPopOut = true;
            MaxSpeedExceedEffectAction?.Invoke(true);
            PlayExtraSoundEffectAction?.Invoke("Momentum");
            Invoke(nameof(SetTeleportPopOutBooleanToFalse), teleportPopOutTimer);
        }
    }

    private void OnSlashBoost(Vector2 boostDirection, float boostPower)
    {
        TetsuoData.ConserveMomentum = false;
        rb.AddForce((GetFacingDirection() * -1) * boostDirection * boostPower, ForceMode2D.Force);
        Invoke(nameof(SetConservationMomentum), 1.2f);
    }

    private void OnDestroy()
    {
        FastMovingPlatform.TouchingPlatformAction -= OnLeavingPlatform;
        TeleportAbility.TeleportPopOutAction -= OnTeleportPopOut;
        TetsuoSlashAttack.SlashBoostAction -= OnSlashBoost;
    }

    private void SetConservationMomentum()
    {
        TetsuoData.ConserveMomentum = true;
    }

    public void SetTeleportPopOutBooleanToFalse()
    {
        hasActivatedTeleportPopOut = false;
        CancelInvoke(nameof(SetTeleportPopOutBooleanToFalse));
    }

    public void Move()
    {
        Debug.Log($"Horizontal: {_horizontal} Vertical: {_vertical}");
        // Force based movement with accel and deaccel, courtesy of https://github.com/Dawnosaur/platformer-movement/blob/main/Scripts/PlayerMovement.cs#L255
        var targetSpeed = _horizontal * TetsuoData.MaxRunSpeed;
        
        // Smooths the change of speed and direction
        targetSpeed = Mathf.Lerp(rb.velocity.x, targetSpeed, TetsuoData.RunLerpAmount);

        float acceleration;

        if (_isGrounded)
        {
            acceleration = Mathf.Abs(targetSpeed) > 0.01 ? TetsuoData.MaxAccelSpeed : TetsuoData.MaxDeAccelSpeed;
        }
        else
        {
            acceleration = Mathf.Abs(targetSpeed) > 0.01 ? TetsuoData.MaxAccelSpeed * TetsuoData.MaxAirAccelSpeed : TetsuoData.MaxDeAccelSpeed * TetsuoData.MaxDeAccelAirSpeed;
        }
        
        // Increase air acceleration and maxSpeed when at the apex of their jump, makes the jump feel a bit more bouncy, responsive and natural
        if ((_isJumping || _isJumpFalling) && Mathf.Abs(rb.velocity.y) < TetsuoData.JumpHangTime)
        {
            acceleration *= 1.1f;
            targetSpeed *= 1.3f;
        }
        
        // Conserving Momentum
        if (TetsuoData.ConserveMomentum && Mathf.Abs(rb.velocity.x) > Mathf.Abs(targetSpeed) &&
            Mathf.Sign(rb.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && !_isGrounded)
        {
            acceleration = 0f;
            // MaxSpeedExceedEffectAction?.Invoke(true);
        }

        // Calculate difference between current velocity and desired velocity
        float speedDif = targetSpeed - rb.velocity.x;
        
        // Calculate force along x-axis to apply to the player
        float movement = speedDif * acceleration;

        // Convert this to a vector and apply to rigidbody
        rb.AddForce(movement * Vector2.right, ForceMode2D.Force);
    }

    public Vector2 GetMovementVector()
    {
        return new Vector2(_horizontal, _vertical);
    }

    private void UpdateJumpFalling()
    {
        if (_isAirDashing)
            return;
        
        if (_isJumping && !_isWallJumping && rb.velocity.y < 0f)
        {
            ClampGravity();
            _isJumping = false;
            _isJumpFalling = true;
        }
    }

    private void UpdateIsGrounded()
    {
        _isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
        if (_isGrounded)
        {
            _isWallSticking = false;
            _isJumpFalling = false;
            TetsuoData._WallStickingTimer = 2f;
            TetsuoData.coyoteTimeCounter = TetsuoData.coyoteTime;
        }
        else
        {
            _hasLanded = false;
            TetsuoData.coyoteTimeCounter-= Time.deltaTime;
        }

        Debug.Log("Is Ground: " + _isGrounded);
    }

    private void UpdateIsFalling()
    {
        if (_isAirDashing)
            return;
        
        if (!_isWallSliding && !_isWallSticking && !_isGrounded && rb.velocity.y < 0)
        {
            _isFalling = true;
            ClampGravity();
        }
        else
        {
            SetGravityScale(_originalGravityScale);
            _isFalling = false;
        }
    }
    
    private void UpdateIsRunning()
    {
        if ((_horizontal is > 0f or < 0f) && Mathf.Abs(rb.velocity.x) >= 0f && _isGrounded)
        {
            _isRunning = true;
        }
        else
        {
            _isRunning = false;
        }
        
        PlayRunSoundEffectAction?.Invoke(_isRunning);
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
            
            JumpOrLandEffectAction?.Invoke(true);
            JumpOrLandEffectAction?.Invoke(false);
            
            PlaySoundEffectAction?.Invoke("Land");
        }
    }

    private void SetGravityScale(float newGravityScale)
    {
        rb.gravityScale = newGravityScale;
    }
    
    private void ClampGravity()
    {
        if (!_isAirDashing)
        {
            SetGravityScale(_originalGravityScale * TetsuoData.FallGravityMultiplier);
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -TetsuoData.MaxFallSpeed));
        }
    }
    
    private bool IsTouchingWall()
    {
        return Physics2D.OverlapBox(wallCheckPoint.position, new Vector2(0.05099699f, 0.7018313f), 0, TetsuoData.wallLayer, 0, 0);
    }

    private void StickToWall()
    {
        if (IsTouchingWall() && !_isGrounded && _horizontal != 0 && TetsuoData._WallStickingTimer > 0f)
        {
            _isWallSticking = true;
            _isWallSliding = false;
            rb.gravityScale = 0f;
            rb.velocity = Vector2.zero;
            _sprite.color = Color.Lerp(_sprite.color, _outOfStaminaColor, Time.deltaTime / TetsuoData._WallStickingTimer);
            TetsuoData._WallStickingTimer -= Time.deltaTime;
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
        Debug.Log($"Is Touching Wall: {IsTouchingWall()}");
        if (IsTouchingWall() && _horizontal != 0 && !_isGrounded && !_isWallSticking) // if you are falling and are running towards the wall
        {
            _isWallSliding = true;
            WallSlidingEffectAction?.Invoke(_isWallSliding, _isFacingRight);
            rb.velocity = new Vector2(rb.velocity.x, Math.Clamp(rb.velocity.y, -TetsuoData.wallSlideSpeed, float.MaxValue));
        }
        else
        {
            _isWallSliding = false;
            WallSlidingEffectAction?.Invoke(_isWallSliding, _isFacingRight);
        }
    }

    private void CheckIfPlayerCanWallJump()
    {
        if (!_isGrounded && IsTouchingWall())
        {
            _isWallJumping = false;
            wallJumpFacingDirection = -transform.localScale.x;
            wallJumpingCounter = TetsuoData.wallJumpingTime;
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }
    }
    
    private void StopWallJumping()
    {
        _isWallJumping = false;
        CancelInvoke(nameof(StopWallJumping));
    }

    private void Jump()
    {
        if (_isWallJumping || _isAirDashing || _isWallSliding || _isWallSticking)
            return;

        if (hasActivatedTeleportPopOut)
        {
            if (Math.Abs(_horizontal) > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x + (2.1f * _horizontal), 17.5f);
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, 20f);
            }
            
            return;
        }
        
        if (hasPressedJump)
        {
            jumpBufferCounter = TetsuoData.jumpBufferTime;
            hasPressedJump = false;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }
        
        if (jumpBufferCounter > 0f && TetsuoData.coyoteTimeCounter > 0f)
        {
            Debug.Log($"Jumped. Buffer: {jumpBufferCounter} Coyote Time {TetsuoData.coyoteTimeCounter}");
            JumpOrLandEffectAction?.Invoke(true);
            PlaySoundEffectAction?.Invoke("Jump");
            _isJumping = true;
            rb.velocity = new Vector2(rb.velocity.x, TetsuoData._jumpingPower);
            jumpBufferCounter = 0f;
        }
        
        if (JumpBoostButton.WasReleasedThisFrame() && rb.velocity.y > 0)
        {
            hasReleasedJump = false;
            _isJumping = true;
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            jumpBufferCounter = 0f;
            TetsuoData.coyoteTimeCounter = 0f;
        }
        
        JumpOrLandEffectAction?.Invoke(false);
    }
    
    private void ModifyJumpPeakGravity()
    {
        if (!_isAirDashing && (_isJumping || _isJumpFalling) && Mathf.Abs(rb.velocity.y) < 0.2f)
        {
            SetGravityScale(_originalGravityScale * 0.5f);
        }
        else if(!_isAirDashing && _isFalling)
        {
            SetGravityScale(_originalGravityScale * TetsuoData.FallGravityMultiplier);
        }
    }

    private IEnumerator Dash()
    {
        Debug.Log("Dash");
        var dashCoolDown = TetsuoData.dashCooldown;
        SetDashParameters();
        
        rb.velocity = Vector2.zero;
        _isAirDashing = true;
        SetGravityScale(0f);
        PlaySoundEffectAction?.Invoke("Dash");
        yield return new WaitForSeconds(TetsuoData.dashTime + 0.4f);
        
        var dashDirection = _horizontal != 0 || _vertical != 0
            ? new Vector2( _horizontal, _vertical).normalized * TetsuoData.dashPower
            : new Vector2(TetsuoData.dashPower * transform.localScale.x, 0f);
        rb.velocity = dashDirection;
        _inputAction.enabled = false;
        DashAttackLeafEffectAction?.Invoke(true);
        groundedDashAttackEvent?.Invoke(this, groundedDashAttackEventArgs);
        yield return new WaitForSeconds(TetsuoData.dashTime);
        
        DashAttackLeafEffectAction?.Invoke(false);
        _isAirDashing = false;
        var startTime = Time.time;
        rb.velocity = TetsuoData.dashEndSpeed * dashDirection.normalized;

        while (Time.time - startTime <= TetsuoData.dashEndTime)
        {
            yield return null;
        }
        
        _inputAction.enabled = true;
        SetGravityScale(_originalGravityScale);
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

    private void SetDashParameters()
    {
        _doneDashing = false;
        _hasDashed = true;
        _sprite.color = new Color(Color.cyan.r, Color.cyan.g, Color.cyan.b, 150); // Temp color
    }

    private void Flip()
    {
        _isFacingRight = !_isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    public float GetFacingDirection()
    {
        return transform.localScale.x;
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

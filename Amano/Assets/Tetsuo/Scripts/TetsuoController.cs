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
    private float _jumpingPower = 8f;
    private bool _isFacingRight = true;
    private SpriteRenderer _sprite;

    void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        rb.velocity = new Vector2(_horizontal * _speed, rb.velocity.y);
        SetAnimatorState();

        switch (_isFacingRight)
        {
            case false when _horizontal > 0f:
            case true when _horizontal < 0f:
                Flip();
                break;
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
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
        _sprite.flipX = !_isFacingRight;
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
        if (_horizontal is > 0f or < 0f)
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
    }
}

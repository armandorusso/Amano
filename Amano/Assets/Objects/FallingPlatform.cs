using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    [SerializeField] public float PlatformFallSpeed;
    [SerializeField] public float DelayBeforePlatformFalls;
    [SerializeField] public float RespawnTime;
    private Vector2 RespawnLocation;
    private Color _originalSpriteColor;
    private Color _fadedSpriteColor;
    private SpriteRenderer _fallingPlatformSprite;
    private Rigidbody2D _rb;
    private BoxCollider2D _collider;
    private bool _steppedOn;
    private bool _isPlatformBreaking;
    private bool _isFalling;
    private bool _canRespawn;
    
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _fallingPlatformSprite = GetComponent<SpriteRenderer>();
        _collider = GetComponent<BoxCollider2D>();
        _rb.bodyType = RigidbodyType2D.Static;
        _originalSpriteColor = _fallingPlatformSprite.color;
        _fadedSpriteColor = new Color(_originalSpriteColor.r, _originalSpriteColor.g, _originalSpriteColor.b, 0);
        RespawnLocation = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (_steppedOn && !_isFalling)
        {
            Invoke(nameof(DelayPlatformFalling), DelayBeforePlatformFalls);
            _isPlatformBreaking = true;
            _steppedOn = false;
        }

        else if(_isPlatformBreaking)
        {
            _fallingPlatformSprite.color = Color.Lerp(_fallingPlatformSprite.color, _fadedSpriteColor,
                Time.deltaTime / DelayBeforePlatformFalls);
        }
        
        else if (_canRespawn)
        {
            RespawnPlatform();
        }
    }

    private void DelayPlatformFalling()
    {
        _isFalling = true;
        // _rb.bodyType = RigidbodyType2D.Dynamic;
        // _rb.gravityScale = PlatformFallSpeed;
        Invoke(nameof(StartRespawnPlatform), RespawnTime);
    }

    private void StartRespawnPlatform()
    {
        _isPlatformBreaking = false;
        _canRespawn = true;
    }
    
    private void RespawnPlatform()
    {
        transform.position = Vector3.MoveTowards(transform.position, RespawnLocation, Time.deltaTime / 1f);
        _fallingPlatformSprite.color = Color.Lerp(_fallingPlatformSprite.color, _originalSpriteColor,
            Time.deltaTime / DelayBeforePlatformFalls);
        ResetPlatformValues();
    }

    private void ResetPlatformValues()
    {
        _isFalling = false;
        _rb.bodyType = RigidbodyType2D.Static;
        _rb.gravityScale = 0f;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            _steppedOn = true;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    [SerializeField] public float PlatformFallSpeed;
    [SerializeField] public float DelayBeforePlatformFalls = 2f;
    private Rigidbody2D _rb;
    private bool steppedOn;
    private bool isFalling;
    
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.bodyType = RigidbodyType2D.Static;
    }

    // Update is called once per frame
    void Update()
    {
        if (steppedOn && !isFalling)
        {
            Invoke(nameof(DelayPlatformFalling), DelayBeforePlatformFalls);
            steppedOn = false;
        }
    }

    private void DelayPlatformFalling()
    {
        isFalling = true;
        _rb.bodyType = RigidbodyType2D.Dynamic;
        _rb.gravityScale = PlatformFallSpeed;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            steppedOn = true;
        }
    }
}

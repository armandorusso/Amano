using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ShurikenProjectile : MonoBehaviour
{
    private Animator _animator;
    private Rigidbody2D rb;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == 8)
        {
            Assert.IsNotNull(rb);
            Assert.IsNotNull(_animator);
            
            _animator.enabled = false;
            rb.bodyType = RigidbodyType2D.Static;
        }
    }
}

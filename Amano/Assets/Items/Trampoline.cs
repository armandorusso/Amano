using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MonoBehaviour
{
    [SerializeField] private float _jumpForceAmount;
    [SerializeField] private Vector2 _launchDirection;

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player") && col.gameObject.TryGetComponent(out Rigidbody2D rb))
        {
            rb.AddForce(_launchDirection * _jumpForceAmount, ForceMode2D.Impulse);
        }
    }
}

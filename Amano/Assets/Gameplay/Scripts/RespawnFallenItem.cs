using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnFallenItem : MonoBehaviour
{
    private Vector2 _itemSpawnPosition;
    private Rigidbody2D _rb;
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _itemSpawnPosition = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("DeathZone"))
        {
            ResetItemPosition();
            ResetItemVelocity();
        }
    }

    private void ResetItemPosition()
    {
        transform.position = _itemSpawnPosition;
    }

    private void ResetItemVelocity()
    {
        _rb.velocity = Vector2.zero;
    }
}

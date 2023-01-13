using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TetsuoOneWayPlatforms : MonoBehaviour
{
    [SerializeField] private BoxCollider2D _playerCollider;
    private IMove _moveableCharacter;
    private GameObject _platform;

    private void Start()
    {
        _moveableCharacter = gameObject.GetComponent(typeof(IMove)) as IMove;
    }
    
    void Update()
    {
        if (_moveableCharacter.GetMovementVector().y <= -1 && _platform)
        {
            StartCoroutine(DisablePlayerCollision());
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Platform"))
        {
            _platform = col.gameObject;
        }
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Platform"))
        {
            _platform = null;
        }
    }

    private IEnumerator DisablePlayerCollision()
    {
        var platformCollider = _platform.GetComponent<CompositeCollider2D>();
        Physics2D.IgnoreCollision(_playerCollider, platformCollider);
        yield return new WaitForSeconds(0.25f);
        Physics2D.IgnoreCollision(_playerCollider, platformCollider, false);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TetsuoOneWayPlatforms : MonoBehaviour
{
    [SerializeField] private BoxCollider2D _playerCollider;
    private TetsuoController _moveableCharacter;
    private GameObject _platform;

    private void Start()
    {
        _moveableCharacter = GetComponent<TetsuoController>();
    }
    
    void Update()
    {
        if (_moveableCharacter.GetMovementVector().y == -1 && _platform)
        {
            _platform.GetComponent<PlatformEffector2D>().rotationalOffset = 180f;
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
            _platform.GetComponent<PlatformEffector2D>().rotationalOffset = 0f;
            _platform = null;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Transform _Start;
    [SerializeField] private Transform _End;
    [SerializeField] private float _speed;
    [SerializeField] private float _delayBetweenMoveBack;
    [SerializeField] private bool isSpike;
    
    private Transform _endPoint;
    private Transform _newPoint;
    private bool _canMovePlatform;
    
    private void Start()
    {
        _endPoint = _End;
        _newPoint = _Start;
        _canMovePlatform = true;
    }

    void Update()
    {
        if (_canMovePlatform)
        {
            transform.position = Vector2.MoveTowards(transform.position, _endPoint.position, Time.deltaTime * _speed);
        }

        if (Vector2.Distance(transform.position, _endPoint.position) <= 0.5f)
        {
            SwapPoints();
        }
    }

    private void SwapPoints()
    {
        StartCoroutine(DelayPlatformMoveBack());
    }

    private IEnumerator DelayPlatformMoveBack()
    {
        var temp = _endPoint;
        _endPoint = _newPoint;
        _newPoint = temp;
        _canMovePlatform = false;
        yield return new WaitForSeconds(_delayBetweenMoveBack);
        _canMovePlatform = true;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!isSpike && col.gameObject.CompareTag("Player") || col.gameObject.CompareTag("Teleport"))
        {
            col.gameObject.GetComponent<Rigidbody2D>().interpolation = RigidbodyInterpolation2D.None;
            
            col.collider.transform.SetParent(transform);
        }
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        if (!isSpike && col.gameObject.CompareTag("Player") || col.gameObject.CompareTag("Teleport"))
        {
            col.collider.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (!isSpike && col.gameObject.CompareTag("Player") || col.gameObject.CompareTag("Teleport"))
        {
            col.gameObject.transform.SetParent(null);

        }
    }
}
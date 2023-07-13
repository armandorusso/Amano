using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Transform _Start;
    [SerializeField] private Transform _End;
    [SerializeField] private float _speed;
    [SerializeField] private bool isSpike;
    
    private Transform _endPoint;
    private Vector2 _previousPosition;
    private Transform _newPoint;
    private Rigidbody2D _rb;
    public static Action<Vector2> TouchingPlatformAction;
    
    private void Start()
    {
        _endPoint = _End;
        _newPoint = _Start;
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        _previousPosition = transform.position;
        transform.position = Vector2.Lerp(transform.position, _endPoint.position, Time.deltaTime * _speed);
        if (Vector2.Distance(transform.position, _endPoint.position) <= 0.5f)
        {
            var temp = _endPoint;
            _endPoint = _newPoint;
            _newPoint = temp;
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!isSpike && col.gameObject.CompareTag("Player"))
        {
            col.gameObject.GetComponent<Rigidbody2D>().interpolation = RigidbodyInterpolation2D.None;
            
            col.collider.transform.SetParent(transform);
        }
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        if (!isSpike && col.gameObject.CompareTag("Player"))
        {
            col.collider.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D col)
    {
 
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveableObstacle : MonoBehaviour
{
    [SerializeField] private Transform _point1;
    [SerializeField] private Transform _point2;
    [SerializeField] private float _speed;
    [SerializeField] private float _delayBetweenPoints;
    [SerializeField] private bool IsFastPlatform;
    
    private Transform _endPoint;
    private Vector2 _previousPosition;
    private Transform _newPoint;
    private Rigidbody2D _rb;
    public static Action<Vector2> TouchingPlatformAction;
    
    private void Start()
    {
        _endPoint = _point2;
        _newPoint = _point1;
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
            // Coroutine instead?
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            col.gameObject.GetComponent<Rigidbody2D>().interpolation = RigidbodyInterpolation2D.None;
            
            col.collider.transform.SetParent(transform);
        }
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            col.collider.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            col.gameObject.GetComponent<Rigidbody2D>().interpolation = RigidbodyInterpolation2D.Extrapolate;
            var platformVelocity = ((((Vector2) transform.position - _previousPosition)) / Time.deltaTime) * 1.5f;
            
            if(IsFastPlatform)
                TouchingPlatformAction?.Invoke(platformVelocity);
            
            col.gameObject.transform.SetParent(null);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Internal;

public class FastMovingPlatform : MonoBehaviour
{
    [SerializeField] private Transform _startPoint;
    [SerializeField] private Transform _endPoint;
    [SerializeField] private float _speed;
    [SerializeField] private float _moveBackSpeed;
    [SerializeField] private float _moveBackDelay;

    private Vector2 _previousPosition;
    private Rigidbody2D _rb;
    private bool isPlatformActive;
    public static Action<Vector2> TouchingPlatformAction;
    private bool _hasLeftPlatform;
    
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        _previousPosition = transform.position;
    }

    private void FixedUpdate()
    {
        if (isPlatformActive)
        {
            MovePlatform();
        }
    }

    private void MovePlatform()
    {
        transform.position = Vector2.MoveTowards(transform.position, _endPoint.position, Time.deltaTime * (_speed * 1.25f));
        
        if (Vector2.Distance(transform.position, _endPoint.position) <= 0.5f)
        {
            isPlatformActive = false;
            StartCoroutine(MovePlatformBackToOriginalPosition());
        }
    }

    private IEnumerator MovePlatformBackToOriginalPosition()
    {
        // Delay before moving back
        yield return new WaitForSeconds(_moveBackDelay);

        // Move platform back to original position once delay is over
        while (Vector2.Distance(transform.position, _startPoint.position) >= 0.5f)
        {
            transform.position = Vector2.MoveTowards(transform.position, _startPoint.position, Time.deltaTime * _moveBackSpeed);
            yield return null;
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            _hasLeftPlatform = false;
            
            col.gameObject.GetComponent<Rigidbody2D>().interpolation = RigidbodyInterpolation2D.None;
            
            col.collider.transform.SetParent(transform);
        }

        if (col.gameObject.CompareTag("Shuriken"))
        {
            isPlatformActive = true;
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
        if (col.gameObject.CompareTag("Player") && !_hasLeftPlatform)
        {
            col.gameObject.transform.SetParent(null);
            _hasLeftPlatform = true;
            col.gameObject.GetComponent<Rigidbody2D>().interpolation = RigidbodyInterpolation2D.Extrapolate;
            var platformVelocity = ((((Vector2) transform.position - _previousPosition)) / Time.deltaTime) * 1.5f;

            if(isPlatformActive)
                TouchingPlatformAction?.Invoke(platformVelocity);
        }
    }
}

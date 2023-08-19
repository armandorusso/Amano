using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastMovingPlatform : MonoBehaviour
{
    [SerializeField] private Transform _startPoint;
    [SerializeField] private Transform _endPoint;
    [SerializeField] private float _speed;

    private Vector2 _previousPosition;
    private Rigidbody2D _rb;
    private bool isPlatformActive;
    public static Action<Vector2> TouchingPlatformAction;
    
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        _previousPosition = transform.position;
        
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
            StartCoroutine(DelayPlatformFromMovingBack());
        }
    }

    private IEnumerator DelayPlatformFromMovingBack()
    {
        yield return new WaitForSeconds(1f);

        while (Vector2.Distance(transform.position, _startPoint.position) >= 0.5f)
        {
            transform.position = Vector2.MoveTowards(transform.position, _startPoint.position, Time.deltaTime * 1f);
            yield return null;
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
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
        if (col.gameObject.CompareTag("Player"))
        {
            col.gameObject.GetComponent<Rigidbody2D>().interpolation = RigidbodyInterpolation2D.Extrapolate;
            var platformVelocity = ((((Vector2) transform.position - _previousPosition)) / Time.deltaTime) * 1.5f;
            
            if(isPlatformActive)
                TouchingPlatformAction?.Invoke(platformVelocity);
            
            col.gameObject.transform.SetParent(null);
        }
    }
}

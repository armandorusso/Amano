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
    
    private Transform _endPoint;
    private Transform _newPoint;
    
    private void Start()
    {
        _endPoint = _point2;
        _newPoint = _point1;
    }

    void Update()
    {
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
            col.gameObject.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            col.gameObject.transform.SetParent(null);
        }
    }
}

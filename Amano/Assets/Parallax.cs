using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public Camera camera;
    public Transform player;
    private Vector2 startPosition;
    private float startZ;

    private Vector2 travel => (Vector2) camera.transform.position - startPosition; // Distance camera has moved from original position of sprite
    private Vector2 parallaxFactor;

    private void Start()
    {
        startPosition = transform.position;
        startZ = transform.position.z;
    }

    private void Update()
    {
        transform.position = startPosition + travel;
    }
}

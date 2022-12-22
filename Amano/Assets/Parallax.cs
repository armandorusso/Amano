using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] public Camera camera;
    [SerializeField] public Transform player;
    private Vector2 startPosition;
    private float startZ;

    private Vector2 travel => (Vector2) camera.transform.position - startPosition; // Distance camera has moved from original position of background

    private float distanceFromPlayer => transform.position.z - player.position.z;

    private float clippingPlane => (camera.transform.position.z +
                                    (distanceFromPlayer > 0 ? camera.farClipPlane : camera.nearClipPlane));
    private float parallaxFactor => Mathf.Abs(distanceFromPlayer) / clippingPlane;

    private void Start()
    {
        startPosition = transform.position;
        startZ = transform.position.z;
    }

    private void Update()
    {
        var pos = transform.position = startPosition + travel * parallaxFactor;
        transform.position = new Vector3(pos.x, pos.y, startZ);
    }
}

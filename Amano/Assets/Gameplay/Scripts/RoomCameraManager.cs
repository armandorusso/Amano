using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCameraManager : MonoBehaviour
{
    [SerializeField] public GameObject VirtualCamera;

    public void OnTriggerEnter2D(Collider2D col)
    {
        if(col.CompareTag("Player"))
            VirtualCamera.SetActive(true);
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
            VirtualCamera.SetActive(false);
    }
}

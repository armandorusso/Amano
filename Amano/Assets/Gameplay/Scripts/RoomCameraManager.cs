using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class RoomCameraManager : MonoBehaviour
{
    [SerializeField] public GameObject VirtualCamera;
    [SerializeField] private CinemachineBrain _cmBrain;
    private CinemachineVirtualCamera _camera; // Change transition speed parameters

    private void Start()
    {
        _cmBrain.m_IgnoreTimeScale = true;
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            VirtualCamera.SetActive(true);
            StartCoroutine(WaitUntilBlendEnds());
        }
    }

    private IEnumerator WaitUntilBlendEnds()
    {
        Debug.Log("Entered Coroutine");
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(2f);
        Time.timeScale = 1f;
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
            VirtualCamera.SetActive(false);
    }
}

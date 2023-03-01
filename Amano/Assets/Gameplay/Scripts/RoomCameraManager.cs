using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class RoomCameraManager : MonoBehaviour
{
    [SerializeField] public GameObject VirtualCamera;
    [SerializeField] private CinemachineBrain _cmBrain;
    [SerializeField] public float ZoomInAmount;
    [SerializeField] private CinemachineVirtualCamera _camera; // Change transition speed parameters
    public class CameraTransitionArgs : EventArgs
    {
        public bool isMovementDisabled { get; set; }
    }
    public static event EventHandler<CameraTransitionArgs> cameraTransitionEvent;
    private CameraTransitionArgs cameraTransitionArgs;
    

    private void Start()
    {
        _cmBrain.m_IgnoreTimeScale = true;
        cameraTransitionArgs = new CameraTransitionArgs
        {
            isMovementDisabled = false
        };
        DontDestroyOnLoad(VirtualCamera);
        QuickTimeTeleport.ZoomInEvent += OnQuickTimeEvent;
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
        cameraTransitionEvent?.Invoke(this, cameraTransitionArgs);
        yield return new WaitForSecondsRealtime(2f);
        cameraTransitionArgs.isMovementDisabled = true;
        cameraTransitionEvent?.Invoke(this, cameraTransitionArgs);
        Time.timeScale = 1f;
    }

    private void OnQuickTimeEvent(object sender, QuickTimeTeleport.ZoomInEventArgs e)
    {
        var duration = 0f;
        while (duration < 2f)
        {
            _camera.m_Lens.OrthographicSize = Mathf.Lerp(_camera.m_Lens.OrthographicSize, e.zoomInAmount, 0.8f);
            duration += Time.deltaTime;
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
            VirtualCamera.SetActive(false);
    }

    public void OnDestroy()
    {
        QuickTimeTeleport.ZoomInEvent -= OnQuickTimeEvent;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class RoomCameraManager : MonoBehaviour
{
    [SerializeField] public GameObject VirtualCamera;
    [SerializeField] private CinemachineBrain _cmBrain;
    [SerializeField] public float OriginalFieldOfView;
    [SerializeField] public float Velocity;
    [SerializeField] public float SmoothTime;
    [SerializeField] private CinemachineVirtualCamera _camera; // Change transition speed parameters

    private bool _hasClickedStart;
    
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
        StartGame.ZoomOutCameraAction += OnStartGameEvent;
    }

    private void Update()
    {
        if (_hasClickedStart)
        {
            _camera.m_Lens.OrthographicSize =
                Mathf.SmoothDamp(_camera.m_Lens.OrthographicSize, OriginalFieldOfView, ref Velocity, SmoothTime);

            if (Mathf.Abs(_camera.m_Lens.OrthographicSize - OriginalFieldOfView) <= 0.2f)
            {
                _hasClickedStart = false;
            }
        }
    }

    private void OnStartGameEvent(bool hasStartedGame, float zoomOutTime)
    {
        _hasClickedStart = hasStartedGame;
        SmoothTime = zoomOutTime - SmoothTime;
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player") && _cmBrain.ActiveVirtualCamera.VirtualCameraGameObject != VirtualCamera)
        {
            VirtualCamera.SetActive(true);
            StartCoroutine(WaitUntilBlendEnds());
        }
    }

    private IEnumerator WaitUntilBlendEnds()
    {
        Debug.Log("Entered Coroutine");
        Time.timeScale = 0.05f;
        cameraTransitionArgs.isMovementDisabled = false;
        cameraTransitionEvent?.Invoke(this, cameraTransitionArgs);
        yield return new WaitForSecondsRealtime(0.5f);
        cameraTransitionArgs.isMovementDisabled = true;
        cameraTransitionEvent?.Invoke(this, cameraTransitionArgs);
        Time.timeScale = 1f;
    }
    

    public void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player") && _cmBrain.ActiveVirtualCamera.VirtualCameraGameObject != VirtualCamera)
            VirtualCamera.SetActive(false);
    }
}

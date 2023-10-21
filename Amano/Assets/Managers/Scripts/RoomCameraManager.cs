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
    [SerializeField] public float _delayUserInput = 0.58f;
    [SerializeField] private CinemachineVirtualCamera _camera; // Change transition speed parameters

    private bool _hasClickedStart;
    private bool _hasEndedLevel;
    private bool _isOffsetTriggered;
    private AimDirectionTracker _aimTracker;
    private CinemachineFramingTransposer _transposer;
    private float _originalDeadZoneWidth;
    private float _originalDeadZoneHeight;
    private float _originalSoftzoneWidth;
    private float _originalSoftzoneHeight;

    public static Action<bool> ShowCameraIconAction;

    public class CameraTransitionArgs : EventArgs
    {
        public bool isMovementEnabled { get; set; }
    }
    public static event EventHandler<CameraTransitionArgs> cameraTransitionEvent;
    private CameraTransitionArgs cameraTransitionArgs;

    private void Start()
    {
        _cmBrain.m_IgnoreTimeScale = true;
        cameraTransitionArgs = new CameraTransitionArgs
        {
            isMovementEnabled = false
        };
        StartGame.ZoomOutCameraAction += OnStartGameEvent;
        LevelManager.ZoomInCameraAction += OnEndLevelEvent;
        TetsuoZoom.ZoomTriggeredAction += OnZoomButtonTriggered;
    }

    private void OnZoomButtonTriggered(bool isOffsetTriggered, AimDirectionTracker aimTracker)
    {
        _isOffsetTriggered = isOffsetTriggered;
        _aimTracker = aimTracker;
        
        ShowCameraIconAction?.Invoke(_isOffsetTriggered);
        
        _transposer = _camera.GetCinemachineComponent<CinemachineFramingTransposer>();

        if (_isOffsetTriggered)
        {
            _originalDeadZoneWidth = _transposer.m_DeadZoneWidth;
            _originalDeadZoneHeight = _transposer.m_DeadZoneHeight;
            _originalSoftzoneWidth = _transposer.m_SoftZoneWidth;
            _originalSoftzoneHeight = _transposer.m_SoftZoneHeight;
        }

        if (!_isOffsetTriggered)
        {
            ResetCamera();
        }
    }

    private void Update()
    {
        if (_hasClickedStart || _hasEndedLevel)
        {
            _camera.m_Lens.OrthographicSize =
                Mathf.SmoothDamp(_camera.m_Lens.OrthographicSize, OriginalFieldOfView, ref Velocity, SmoothTime);

            if (Mathf.Abs(_camera.m_Lens.OrthographicSize - OriginalFieldOfView) <= 0.2f)
            {
                _hasClickedStart = false;
                _hasEndedLevel = false;
            }
        }
        
        ApplyOffset();
    }

    private void ApplyOffset()
    {
        if (_isOffsetTriggered)
        {
            Debug.Log("Offset Activated");
            var inputVector = _aimTracker.CurrentInput == GameInputManager.InputType.Controller ? _aimTracker.GetRightStickDirection(): (Vector2) _aimTracker.GetMousePositionInScreen().normalized;
            
            // Set the deadzone values so the offset is more noticeable
            _transposer.m_DeadZoneWidth = 0f;
            _transposer.m_DeadZoneHeight = 0f;

            _transposer.m_SoftZoneWidth = 0f;
            _transposer.m_SoftZoneHeight = 0f;

            // Move camera in the x and y direction depending on the input provided
            Debug.Log($"right: {transform.up}");
            var moveDirection = transform.right * inputVector.x + transform.up * inputVector.y;

            var moveSpeed = 8f;
            _transposer.m_TrackedObjectOffset += moveDirection * moveSpeed * Time.deltaTime;
            _transposer.m_TrackedObjectOffset.x = Mathf.Clamp(_transposer.m_TrackedObjectOffset.x, -5f, 5f);
            _transposer.m_TrackedObjectOffset.y = Mathf.Clamp(_transposer.m_TrackedObjectOffset.y, -5f, 5f);
        }
    }

    private void ResetCamera()
    {
        Debug.Log("Offset Deactivated");
        _transposer.m_TrackedObjectOffset = Vector3.zero;
        
        _transposer.m_DeadZoneWidth = _originalDeadZoneWidth;
        _transposer.m_DeadZoneHeight = _originalDeadZoneHeight;

        _transposer.m_SoftZoneWidth = _originalSoftzoneWidth;
        _transposer.m_SoftZoneHeight = _originalSoftzoneHeight;

        _camera.transform.position -= _transposer.m_TrackedObjectOffset;
    }

    private void OnStartGameEvent(bool hasStartedGame, float zoomOutTime)
    {
        _hasClickedStart = hasStartedGame;
        SmoothTime = zoomOutTime - SmoothTime;
    }
    
    private void OnEndLevelEvent()
    {
        _hasEndedLevel = true;
        OriginalFieldOfView = 2f;
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player") && _cmBrain.ActiveVirtualCamera?.VirtualCameraGameObject != VirtualCamera)
        {
            VirtualCamera.SetActive(true);
            StartCoroutine(WaitUntilBlendEnds());
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Player") && _cmBrain.ActiveVirtualCamera?.VirtualCameraGameObject != VirtualCamera)
        {
            VirtualCamera.SetActive(false);
            VirtualCamera.SetActive(true);
        }
    }

    private IEnumerator WaitUntilBlendEnds()
    {
        Debug.Log("Entered Coroutine");
        Time.timeScale = 0.05f;
        cameraTransitionArgs.isMovementEnabled = false;
        cameraTransitionEvent?.Invoke(this, cameraTransitionArgs);
        yield return new WaitForSecondsRealtime(_delayUserInput);
        cameraTransitionArgs.isMovementEnabled = true;
        cameraTransitionEvent?.Invoke(this, cameraTransitionArgs);
        Time.timeScale = 1f;
    }
    

    public void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log($"Player: {other == null}");
        Debug.Log($"Camera: {_cmBrain.ActiveVirtualCamera?.VirtualCameraGameObject == null}");
        
        if(other.CompareTag("Player") && _cmBrain.ActiveVirtualCamera?.VirtualCameraGameObject != VirtualCamera)
            VirtualCamera.SetActive(false);
    }

    private void OnDestroy()
    {
        StartGame.ZoomOutCameraAction -= OnStartGameEvent;
        LevelManager.ZoomInCameraAction -= OnEndLevelEvent;
        TetsuoZoom.ZoomTriggeredAction -= OnZoomButtonTriggered;
    }
}

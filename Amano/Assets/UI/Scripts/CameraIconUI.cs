using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraIconUI : MonoBehaviour
{
    [SerializeField] public Image _cameraIconImage;

    private void Awake()
    {
        RoomCameraManager.ShowCameraIconAction += OnCameraPanButtonActivated;
    }
    
    private void OnCameraPanButtonActivated(bool isButtonClicked)
    {
        _cameraIconImage.enabled = isButtonClicked;
    }
}

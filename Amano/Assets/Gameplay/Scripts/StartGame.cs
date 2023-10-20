using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    [SerializeField] public float ZoomOutTime;
    public static Action<bool> FadeStartScreenAction;
    public static Action<bool, float> PlayGetUpAnimationAction;
    public static Action<bool, float> ZoomOutCameraAction;

    public void OnStartGame()
    {
        FadeStartScreenAction?.Invoke(true);
        PlayGetUpAnimationAction?.Invoke(true, ZoomOutTime);
        ZoomOutCameraAction?.Invoke(true, ZoomOutTime);
    }
}

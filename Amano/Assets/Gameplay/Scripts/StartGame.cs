using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    [SerializeField] public float ZoomOutTime;
    // Start is called before the first frame update
    public static Action<bool> FadeStartScreenAction;
    public static Action<bool> PlayGetUpAnimationAction;
    public static Action<bool> ZoomOutCameraAction;

    public void OnStartGame()
    {
        FadeStartScreenAction?.Invoke(true);
        PlayGetUpAnimationAction?.Invoke(true);
        ZoomOutCameraAction?.Invoke(true);
    }
}

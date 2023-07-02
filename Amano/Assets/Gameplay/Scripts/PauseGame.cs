using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseGame : MonoBehaviour
{
    [SerializeField] private GameObject PauseScreen;

    private void Start()
    {
        TetsuoDisableMovement.PauseGameAction += OnClickPause;
    }

    public void OnClickContinue()
    {
        Time.timeScale = 1f;
        PauseScreen.SetActive(false);
        TetsuoDisableMovement.Instance.EnableOrDisableInputActions(true);
    }

    public void OnClickPause()
    {
        PauseScreen.SetActive(true);
    }
}

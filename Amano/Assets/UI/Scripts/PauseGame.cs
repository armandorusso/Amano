using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseGame : MonoBehaviour
{
    [SerializeField] private GameObject PauseScreen;
    [SerializeField] private GameObject SettingsScreen;

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

    public void OnSettingsButtonClicked()
    {
        SettingsScreen.SetActive(true);
        PauseScreen.SetActive(false);
    }
    
    public void OnBackButtonClicked()
    {
        SettingsScreen.SetActive(false);
        PauseScreen.SetActive(true);
    }
}

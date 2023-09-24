using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TitleScreenMenu : MonoBehaviour
{
    [SerializeField] public Button[] TitleScreenButtons;
    private CanvasGroup _titleScreenCanvas;
    private bool _hasStartedGame;
    
    void Start()
    {
        _titleScreenCanvas = GetComponent<CanvasGroup>();
        StartGame.FadeStartScreenAction += OnStartGame;
        
        EventSystem.current.SetSelectedGameObject(TitleScreenButtons[0].gameObject);
    }

    private void Update()
    {
        if (_hasStartedGame)
        {
            FadeOutTitleScreen();
        }

        if (_titleScreenCanvas.alpha <= 0.005f)
        {
            gameObject.SetActive(false);
        }
    }

    private void FadeOutTitleScreen()
    {
        _titleScreenCanvas.alpha = Mathf.Lerp(_titleScreenCanvas.alpha, 0, Time.deltaTime / 1f * 2f);
    }

    private void OnStartGame(bool hasStartedGame)
    {
        _hasStartedGame = hasStartedGame;
        
        foreach(Button button in TitleScreenButtons)
        {
            button.enabled = false;
        }
    }

    private void OnDestroy()
    {
        StartGame.FadeStartScreenAction -= OnStartGame;
    }
}

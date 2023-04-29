using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeOutTitleScreen : MonoBehaviour
{
    [SerializeField] public Button[] TitleScreenButtons;
    private CanvasGroup _titleScreenCanvas;
    private bool _hasStartedGame;
    
    void Start()
    {
        _titleScreenCanvas = GetComponent<CanvasGroup>();
        StartGame.FadeStartScreenAction += OnStartGame;
    }

    private void Update()
    {
        if (_hasStartedGame)
        {
            _titleScreenCanvas.alpha = Mathf.Lerp(_titleScreenCanvas.alpha, 0, Time.deltaTime / 1f * 2f);
        }

        if (_titleScreenCanvas.alpha <= 0.005f)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnStartGame(bool hasStartedGame)
    {
        _hasStartedGame = hasStartedGame;
        
        foreach(Button button in TitleScreenButtons)
        {
            button.enabled = false;
        }
    }
}

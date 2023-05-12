using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    private Slider HealthBar;
    private RectTransform[] HealthUISprites;
    
    private void Awake()
    {
        TetsuoHealthBar.healthUIEvent += OnHealthUiEvent;
        TetsuoAnimatorController.EnableHealthUiAction += OnStartGame;
        HealthBar = GetComponent<Slider>();
        HealthUISprites = GetComponentsInChildren<RectTransform>();
        
        foreach (var sprite in HealthUISprites)
        {
            sprite.gameObject.SetActive(false);
        }
    }

    private void OnStartGame(bool hasStarted)
    {
        foreach (var sprite in HealthUISprites)
        {
            sprite.gameObject.SetActive(hasStarted);
        }
    }

    private void OnHealthUiEvent(object sender, TetsuoHealthBar.HealthUIEventArgs e)
    {
        HealthBar.value = e.currentHealth;
    }

    private void OnDestroy()
    {
        TetsuoHealthBar.healthUIEvent -= OnHealthUiEvent;
        TetsuoAnimatorController.EnableHealthUiAction -= OnStartGame;
    }
}

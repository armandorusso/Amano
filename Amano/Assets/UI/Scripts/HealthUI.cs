using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    private Slider HealthBar;
    
    private void Awake()
    {
        TetsuoHealthBar.healthUIEvent += OnHealthUiEvent;
        HealthBar = GetComponent<Slider>();
    }

    private void OnHealthUiEvent(object sender, TetsuoHealthBar.HealthUIEventArgs e)
    {
        HealthBar.value = Mathf.Lerp(HealthBar.value, e.currentHealth, HealthBar.value / Time.deltaTime);
    }
}

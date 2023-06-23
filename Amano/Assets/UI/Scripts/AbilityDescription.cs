using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityDescription : MonoBehaviour
{
    [SerializeField] private GameObject AbilityCanvas;
    [SerializeField] private TextMeshProUGUI AbilityInfoText;
    [SerializeField] private Image AbilityButtonSprite;

    void Start()
    {
        AbilityCanvas.SetActive(false);
        AbilityHandler.AbilityInfoTextAction += OnAbilityUnlock;
    }

    private void OnAbilityUnlock(string abilityDescription, Sprite abilityButtonSprite)
    {
        AbilityCanvas.SetActive(true);
        AbilityButtonSprite.sprite = abilityButtonSprite;
        AbilityInfoText.text = abilityDescription;
    }

    public void OnClickContinue()
    {
        AbilityCanvas.SetActive(false);
        TetsuoDisableMovement.Instance.EnableOrDisableInputActions(true);
    }
}

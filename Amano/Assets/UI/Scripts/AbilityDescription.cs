using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AbilityDescription : MonoBehaviour
{
    [SerializeField] private GameObject AbilityCanvas;
    [SerializeField] private TextMeshProUGUI AbilityInfoText;
    [SerializeField] private Image AbilityButtonSprite;
    [SerializeField] private Button ContinueButton;

    public static Action EnableUIAction;
    
    void Start()
    {
        AbilityCanvas.SetActive(false);
        AbilityHandler.AbilityInfoTextAction += OnAbilityUnlock;
    }

    private void OnAbilityUnlock(string abilityDescription, Sprite abilityButtonSprite)
    {
        AbilityCanvas.SetActive(true);
        EventSystem.current.SetSelectedGameObject(ContinueButton.gameObject);
        AbilityButtonSprite.sprite = abilityButtonSprite;
        AbilityInfoText.text = abilityDescription;
    }

    public void OnClickContinue()
    {
        AbilityCanvas.SetActive(false);
        EnableUIAction?.Invoke();
        TetsuoDisableMovement.Instance.EnableOrDisableInputActions(true); // Can maybe be reworked by using an event instead of a direct call
    }
}

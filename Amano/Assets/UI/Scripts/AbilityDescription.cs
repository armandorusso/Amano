using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AbilityDescription : MonoBehaviour
{
    [SerializeField] public GameObject AbilityCanvas;
    [SerializeField] public TextMeshProUGUI AbilityInfoText;
    [SerializeField] public Image AbilityButtonSprite;
    [SerializeField] public Button ContinueButton;

    private Sprite[] _currentInputSprites;
    public static Action EnableUIAction;
    
    void Start()
    {
        AbilityCanvas.SetActive(false);
        AbilityHandler.AbilityInfoTextAction += OnAbilityUnlock;
        GameInputManager.SwitchInputAction += OnInputSwitch;
    }

    private void OnAbilityUnlock(string abilityDescription, Sprite[] abilityButtonSprites)
    {
        AbilityCanvas.SetActive(true);
        EventSystem.current.SetSelectedGameObject(ContinueButton.gameObject);
        _currentInputSprites = abilityButtonSprites;
        AbilityButtonSprite.sprite = GameInputManager.Instance.currentInputType == GameInputManager.InputType.Controller ? abilityButtonSprites[0] : abilityButtonSprites[1];
        AbilityInfoText.text = abilityDescription;
    }

    private void OnInputSwitch(GameInputManager.InputType inputType)
    {
        if(_currentInputSprites != null)
            AbilityButtonSprite.sprite = inputType == GameInputManager.InputType.Controller ? _currentInputSprites[0] : _currentInputSprites[1];
    }

    public void OnClickContinue()
    {
        AbilityCanvas.SetActive(false);
        EnableUIAction?.Invoke();
        TetsuoDisableMovement.Instance.EnableOrDisableInputActions(true); // Can maybe be reworked by using an event instead of a direct call
    }

    private void OnDestroy()
    {
        AbilityHandler.AbilityInfoTextAction -= OnAbilityUnlock;
        GameInputManager.SwitchInputAction -= OnInputSwitch;
    }
}

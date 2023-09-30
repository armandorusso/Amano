using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonInfoAnimation : MonoBehaviour
{
    [SerializeField] public RuntimeAnimatorController  KeyboardAnimator;
    [SerializeField] public RuntimeAnimatorController  ControllerAnimator;

    private Animator _animator;

    void Start()
    {
        GameInputManager.SwitchInputAction += OnInputSwitch;

        _animator = GetComponent<Animator>();
    }

    private void OnInputSwitch(GameInputManager.InputType input)
    {
        _animator.runtimeAnimatorController = input == GameInputManager.InputType.Controller ? ControllerAnimator
            : KeyboardAnimator;
    }

    private void OnDestroy()
    {
        GameInputManager.SwitchInputAction -= OnInputSwitch;
    }
}

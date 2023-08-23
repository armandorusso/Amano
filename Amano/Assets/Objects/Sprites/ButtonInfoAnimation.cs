using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class ButtonInfoAnimation : MonoBehaviour
{
    [SerializeField] private AnimatorController KeyboardAnimator;
    [SerializeField] private AnimatorController ControllerAnimator;

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
}

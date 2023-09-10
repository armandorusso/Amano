using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractButton : MonoBehaviour
{
    [SerializeField] public Sprite ControllerTeleportButton;
    [SerializeField] public Sprite MouseTeleportButton;
    [SerializeField] public Camera MainCamera;
    [SerializeField] public Vector3 ImageOffset;

    private Image _buttonSprite;
    private bool _isRendered;
    private GameObject _currentShuriken;
    private Queue<GameObject> _attachedShuriken;
    
    void Start()
    {
        _buttonSprite = GetComponent<Image>();
        _attachedShuriken = new Queue<GameObject>(20);
        
        GameInputManager.SwitchInputAction += OnInputSwitch;
        ShurikenProjectile.ShurikenHitTeleportObjectAction += OnShurikenAttachedToTeleportableObject;
        TeleportAbility.RemoveInteractButtonAction += OnLastShurikenConsumed;
        TeleportAbility.ShurikenConsumedAction += OnShurikenConsumed;
    }

    private void Update()
    {
        if(_currentShuriken != null)
            _buttonSprite.transform.position = MainCamera.WorldToScreenPoint(_currentShuriken.transform.position + ImageOffset);
    }

    private void OnInputSwitch(GameInputManager.InputType inputType)
    {
        if (inputType == GameInputManager.InputType.Controller)
        {
            _buttonSprite.sprite = ControllerTeleportButton;
        }
        else
        {
            _buttonSprite.sprite = MouseTeleportButton;
        }
    }

    public void OnShurikenAttachedToTeleportableObject(int gameObjectId, GameObject shuriken)
    {
        _isRendered = true;
        _currentShuriken = shuriken;
        _attachedShuriken.Enqueue(_currentShuriken);
        _buttonSprite.enabled = true;
        _buttonSprite.sprite = GameInputManager.Instance.currentInputType == GameInputManager.InputType.KeyboardMouse ? MouseTeleportButton : ControllerTeleportButton;
        _buttonSprite.transform.rotation = Quaternion.identity;
    }

    private void OnShurikenConsumed()
    {
        _currentShuriken = _attachedShuriken.Dequeue();
    }

    private void OnLastShurikenConsumed()
    {
        _buttonSprite.enabled = false;
    }

    private void OnDestroy()
    {
        GameInputManager.SwitchInputAction -= OnInputSwitch;
        ShurikenProjectile.ShurikenHitTeleportObjectAction -= OnShurikenAttachedToTeleportableObject;
        TeleportAbility.RemoveInteractButtonAction -= OnLastShurikenConsumed;
        TeleportAbility.ShurikenConsumedAction -= OnShurikenConsumed;
    }
}

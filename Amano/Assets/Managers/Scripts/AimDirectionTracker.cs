using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class AimDirectionTracker : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private SpriteRenderer _aimReticleSprite;
    [SerializeField] public GameInputManager.InputType CurrentInput;
    [SerializeField] public FactsScriptableObject AbilityFacts;
    private Color _aimOriginalColor;
    private Color _aimColorWithoutAlpha;
    private bool _isAiming;
    private Vector2 _mouseDelta;
    
    private Vector3 mousePositionInWorld { get; set; }
    private Vector2 rightStickDirection { get; set; }

    private void Start()
    {
        GameInputManager.SwitchInputAction += OnControllerInputSwitch;
        _aimOriginalColor = _aimReticleSprite.color;
        _aimColorWithoutAlpha = new Color(_aimReticleSprite.color.r, _aimReticleSprite.color.g, _aimReticleSprite.color.b, 0f);
        _aimReticleSprite.color = _aimColorWithoutAlpha;
    }

    private void OnControllerInputSwitch(GameInputManager.InputType isUsingController)
    {
        CurrentInput = isUsingController;
    }

    public Vector3 GetMousePositionInWorld()
    {
        return mousePositionInWorld;
    }

    public Vector2 GetMousePositionInScreen()
    {
        return camera.WorldToViewportPoint(mousePositionInWorld);
    }

    public Vector2 GetRightStickDirection()
    {
        return rightStickDirection;
    }

    public void OnMoveAimButton(InputAction.CallbackContext context)
    {
        if (AbilityFacts.Facts["Shuriken"] == 0)
        {
            return;
        }
        
        if (CurrentInput == GameInputManager.InputType.KeyboardMouse)
        {
            /*if (MouseMoveEvent.GetPooled().mouseDelta == Vector2.zero)
            {
                Debug.Log("Not aiming");
                _isAiming = false;
                return;
            }*/
            
            mousePositionInWorld = camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            _aimReticleSprite.color = _aimOriginalColor;
            _isAiming = true;
        }
        else
        {
            if (!context.action.triggered)
            {
                Debug.Log("Not aiming");
                _isAiming = false;
                return;
            }
            
            if (context.started || context.performed)
            {
                rightStickDirection = context.ReadValue<Vector2>();
                _aimReticleSprite.color = _aimOriginalColor;
                _isAiming = true;
            }
            else
            {
                _isAiming = false;
            }
        }

        StartCoroutine(FadeReticle());
    }

    private IEnumerator FadeReticle()
    {
        yield return new WaitForSeconds(2f);
        
        float currentTime = 0f;
        float fadeDuration = 1f;

        while (currentTime <= fadeDuration)
        {
            if (_isAiming)
            {
                _aimReticleSprite.color = _aimOriginalColor;
                break;
            }
            
            currentTime += Time.deltaTime;
            _aimReticleSprite.color =
                Color.Lerp(_aimReticleSprite.color, _aimColorWithoutAlpha, currentTime / fadeDuration);
            yield return null;
        }
        // StopCoroutine(FadeReticle());
    }
}

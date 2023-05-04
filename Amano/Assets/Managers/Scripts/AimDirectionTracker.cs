using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
 
public class AimDirectionTracker : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private SpriteRenderer _aimReticleSprite;
    [SerializeField] public GameInputManager.InputType CurrentInput;
    [SerializeField] public FactsScriptableObject AbilityFacts;
    private Color _aimOriginalColor;
    private Color _aimColorWithoutAlpha;
    private bool _isAiming;
    
    private Vector3 mousePositionInWorld { get; set; }
    private Vector2 rightStickDirection { get; set; }

    private void Start()
    {
        GameInputManager.SwitchInputAction += OnControllerInputSwitch;
        _aimOriginalColor = _aimReticleSprite.color;
        _aimColorWithoutAlpha = new Color(_aimReticleSprite.color.r, _aimReticleSprite.color.g, _aimReticleSprite.color.b, 0.05f);
    }

    private void OnControllerInputSwitch(GameInputManager.InputType isUsingController)
    {
        CurrentInput = isUsingController;
    }

    private void Update()
    {
    }

    public Vector3 GetMousePositionInWorld()
    {
        return mousePositionInWorld;
    }

    public Vector2 GetRightStickDirection()
    {
        return rightStickDirection;
    }

    public void OnShootWeapon(InputAction.CallbackContext context)
    {
        if (AbilityFacts.Facts["Shuriken"] == 0)
        {
            return;
        }

        if (!context.action.triggered)
        {
            Debug.Log("Not aiming");
            _isAiming = false;
            return;
        }

        if (CurrentInput == GameInputManager.InputType.KeyboardMouse)
        {
            mousePositionInWorld = camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            _aimReticleSprite.color = _aimOriginalColor;
            _isAiming = true;
        }
        else
        {
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

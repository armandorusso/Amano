using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShurikenProjectileEffects : MonoBehaviour
{
    [SerializeField] public Color ShurikenAttachedColor;
    [SerializeField] public ParticleSystem ShurikenParticleSystem;
    [SerializeField] public AudioClip ShurikenThrowSound;
    [SerializeField] public AudioClip ShurikenAttachedSound;
    [SerializeField] public Sprite ControllerTeleportButton;
    [SerializeField] public Sprite MouseTeleportButton;
    [SerializeField] public SpriteRenderer ButtonSprite;
    
    private AudioSource _audioSource;
    private Color _originalColor;
    private SpriteRenderer _sprite;
    private bool _isInstantiated;

    private void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _audioSource = GetComponent<AudioSource>();
        _originalColor = _sprite.color;
        ButtonSprite.sprite = MouseTeleportButton;
        
        _isInstantiated = true;
    }

    void Start()
    {
        GameInputManager.SwitchInputAction += OnInputSwitch;
        ShurikenProjectile.ShurikenHitTeleportObjectAction += OnShurikenAttachedToTeleportableObject;
    }

    private void OnEnable()
    {
        if (_isInstantiated)
        {
            ButtonSprite.enabled = false;
            _audioSource.PlayOneShot(ShurikenThrowSound);
        }
    }

    private void OnDisable()
    {
        if (_isInstantiated)
        {
            ButtonSprite.enabled = false;
            ShurikenParticleSystem.Stop();
            ChangeToOriginalColor();
        }
    }

    private void OnInputSwitch(GameInputManager.InputType obj)
    {
        if (GameInputManager.Instance.currentInputType == GameInputManager.InputType.Controller)
        {
            ButtonSprite.sprite = ControllerTeleportButton;
        }
        else
        {
            ButtonSprite.sprite = MouseTeleportButton;
        }
    }

    public void OnShurikenAttachedToTeleportableObject(int gameObjectId)
    {
        if(gameObject.GetInstanceID() == gameObjectId)
        {
            ButtonSprite.enabled = true;
            ShurikenParticleSystem.Play();
            _audioSource.PlayOneShot(ShurikenAttachedSound);
            ChangeToNewColor();
        }
    }
    
    public void ChangeToOriginalColor()
    {
        _sprite.color = _originalColor;
    }
    
    public void ChangeToNewColor()
    {
        _sprite.color = ShurikenAttachedColor;
    }
    
}

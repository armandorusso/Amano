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

    private AudioSource _audioSource;
    private Color _originalColor;
    private SpriteRenderer _sprite;
    private bool _isInstantiated;

    private void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _audioSource = GetComponent<AudioSource>();
        _originalColor = _sprite.color;

        _isInstantiated = true;
    }

    void Start()
    {
        ShurikenProjectile.ShurikenHitTeleportObjectAction += OnShurikenAttachedToTeleportableObject;
    }

    private void OnDestroy()
    {
        ShurikenProjectile.ShurikenHitTeleportObjectAction -= OnShurikenAttachedToTeleportableObject;
    }

    private void OnEnable()
    {
        if (_isInstantiated)
        {
            _audioSource.PlayOneShot(ShurikenThrowSound);
        }
    }

    private void OnDisable()
    {
        if (_isInstantiated)
        {
            ShurikenParticleSystem.Stop();
            ChangeToOriginalColor();
        }
    }
    

    public void OnShurikenAttachedToTeleportableObject(int gameObjectId, GameObject shuriken)
    {
        if(gameObject.GetInstanceID() == gameObjectId)
        {
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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryCollectible : MonoBehaviour
{
    [SerializeField] public AudioClip MemoryCollectSound;
    private bool hasCollected = false;
    private Animator _anim;
    private AudioSource _audioSource;
    
    public static Action IncrementCollectibleCountAction;

    private void Start()
    {
        _anim = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player") && !hasCollected)
        {
            hasCollected = true;
            _anim.SetBool("collected", true);
            _audioSource.PlayOneShot(MemoryCollectSound);
            IncrementCollectibleCountAction?.Invoke();
            
            Invoke(nameof(PlayAnimation), 1.5f);
        }
    }

    private void PlayAnimation()
    {
        gameObject.SetActive(false);
    }
}

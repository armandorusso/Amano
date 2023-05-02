using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryCollectible : MonoBehaviour
{
    private bool hasCollected = false;
    private Animator _anim;

    private void Start()
    {
        _anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player") && !hasCollected)
        {
            hasCollected = true;
            _anim.SetBool("collected", true);
            GameManager.Instance.IncrementCollectibleCount();
            
            Invoke(nameof(PlayAnimation), 1.4f);
        }
    }

    private void PlayAnimation()
    {
        gameObject.SetActive(false);
    }
}

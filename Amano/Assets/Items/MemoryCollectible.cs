using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryCollectible : MonoBehaviour
{
    private bool hasCollected = false;
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player") && !hasCollected)
        {
            hasCollected = true;
            GameManager.Instance.IncrementCollectibleCount();
            gameObject.SetActive(false);
        }
    }
}

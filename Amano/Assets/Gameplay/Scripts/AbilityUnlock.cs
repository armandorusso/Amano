using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AbilityUnlock : MonoBehaviour
{
    [SerializeField] public UnityEvent AbilityUnlocked;
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            AbilityUnlocked?.Invoke();
            gameObject.SetActive(false);
        }
    }
}

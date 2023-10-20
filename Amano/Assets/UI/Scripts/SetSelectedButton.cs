using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SetSelectedButton : MonoBehaviour
{
    void Start()
    {
        
    }

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
}

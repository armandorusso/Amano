using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CollectibleUI : MonoBehaviour
{
    private TextMeshProUGUI _collectibleText;
    private bool hasTextUpdated;
    
    void Start()
    {
        _collectibleText = GetComponent<TextMeshProUGUI>();
        GameManager.CollectibleAction += OnCollectibleCollected;
    }

    private void Update()
    {
        if (hasTextUpdated)
        {
            _collectibleText.CrossFadeAlpha(255f, 0.5f, false);
        }
        else
        {
            _collectibleText.CrossFadeAlpha(0f, 0.5f, false);
        }
    }

    private void OnCollectibleCollected(int collectibleCount)
    {
        _collectibleText.text = $"x {collectibleCount}";
        StartCoroutine(PlayTextFade());
    }

    private IEnumerator PlayTextFade()
    {
        hasTextUpdated = true;
        yield return new WaitForSeconds(0.5f);
        hasTextUpdated = false;
    }
}

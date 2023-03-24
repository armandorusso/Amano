using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CollectibleUI : MonoBehaviour
{
    private TextMeshProUGUI _collectibleText;
    
    void Start()
    {
        _collectibleText = GetComponent<TextMeshProUGUI>();
        GameManager.CollectibleAction += OnCollectibleCollected;
    }

    private void OnCollectibleCollected(int collectibleCount)
    {
        _collectibleText.text = $"x {collectibleCount}";
    }
}

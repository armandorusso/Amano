using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleManager : MonoBehaviour
{
    public int CollectibleCount { get; private set; }
    public static Action<int> CollectibleAction;
    
    private void Awake()
    {
        DontDestroyOnLoad(this);

        MemoryCollectible.IncrementCollectibleCountAction += OnCollectibleObtained;
    }

    public void OnCollectibleObtained()
    {
        CollectibleCount++;
        CollectibleAction?.Invoke(CollectibleCount);
    }

    private void OnDestroy()
    {
        MemoryCollectible.IncrementCollectibleCountAction -= OnCollectibleObtained;
    }
}

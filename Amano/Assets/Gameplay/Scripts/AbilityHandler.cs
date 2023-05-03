using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityHandler : MonoBehaviour
{
    [SerializeField] public FactsScriptableObject AbilityFacts;

    public void OnAbilityUnlocked(string AbilityName)
    {
        if (AbilityFacts.Facts[AbilityName] == 1)
        {
            return;
        }
        
        AbilityFacts.Facts[AbilityName] = 1;
        
        Debug.Log($"{AbilityFacts.Facts[AbilityName]}");
    }
}

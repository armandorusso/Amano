using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityHandler : MonoBehaviour
{
    [SerializeField] public FactsScriptableObject AbilityFacts;

    public static Action<string, Sprite> AbilityInfoTextAction;
    public static Action DisableUIAction;

    public void OnAbilityUnlocked(string AbilityName)
    {
        /*if (AbilityFacts.Facts[AbilityName] == 1)
        {
            return;
        }*/
        
        AbilityFacts.Facts[AbilityName] = 1;
        
        Debug.Log($"{AbilityFacts.Facts[AbilityName]}");
        
        AbilityInfoTextAction?.Invoke(AbilityFacts.FactsDescription[AbilityName], AbilityFacts.FactsButtonSprites[AbilityName]);
        DisableUIAction?.Invoke();
    }
}

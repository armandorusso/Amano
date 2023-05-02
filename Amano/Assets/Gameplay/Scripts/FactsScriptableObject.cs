using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FactsScriptableObject", menuName = "ScriptableObject/Facts")]
public class FactsScriptableObject : ScriptableObject
{
    [SerializeField] public GenericDictionary<string, int> SOFacts = new GenericDictionary<string, int>();
    /*public float SOFacts;*/
}

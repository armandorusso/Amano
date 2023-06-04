using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FactsScriptableObject", menuName = "ScriptableObject/AudioFacts")]
public class AudioFactsScriptableObject : ScriptableObject
{
    [SerializeField] public GenericDictionary<string, AudioClip> Facts = new GenericDictionary<string, AudioClip>();
}

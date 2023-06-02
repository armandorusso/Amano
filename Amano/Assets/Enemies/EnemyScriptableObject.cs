using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyScriptableObject", menuName = "ScriptableObject/Enemy")]
public class EnemyScriptableObject : ScriptableObject
{
    public int Health;
    public float Speed;
    public GameObject ItemUsing;
    public AudioFactsScriptableObject EnemyAudioFacts;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NormalSamuraiScriptableObject", menuName = "ScriptableObject/NormalSamurai")]
public class NormalSamuraiScriptableObject : ScriptableObject
{
    public int Health;
    public int Damage;
    public float Speed;
    public GameObject Projectile;
}

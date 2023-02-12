using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyData : MonoBehaviour
{
    [SerializeField] public EnemyScriptableObject EnemyParameters;
    [SerializeField] public AmanoTimer Timer;
    [SerializeField] public GameObject GroundCheck;
    [SerializeField] public Transform TetsuoPosition;
    [SerializeField] public Transform ThrowPosition;
    [SerializeField] public SpriteRenderer Sprite;
    [SerializeField] public int LineOfSightDistance;
    [SerializeField] public LayerMask RayCastLayers;
    public LayerMask GroundLayer;
    public Rigidbody2D Rb;
    public Collider2D Collider;
}

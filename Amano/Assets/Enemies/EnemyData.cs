using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyData : MonoBehaviour
{
    [SerializeField] public AmanoTimer Timer;
    [SerializeField] public GameObject GroundCheck;
    [SerializeField] public Transform TetsuoPosition;
    [SerializeField] public Transform ThrowPosition;
    [SerializeField] public int LineOfSightDistance;
    [SerializeField] public LayerMask RayCastLayers;
    [SerializeField] public EnemyScriptableObject EnemyParameters;
    public LayerMask GroundLayer;
}

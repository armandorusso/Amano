using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetsuoSlashAttack : MonoBehaviour
{
    [SerializeField] public LayerMask HitboxLayerMask;
    [SerializeField] public Vector2 BoostDirection;
    [SerializeField] public float BoostPower;

    public static Action<Vector2, float> SlashBoostAction;

    private void Start()
    {
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        var collidedGameObject = col.gameObject;

        if ((HitboxLayerMask.value & 1 << collidedGameObject.layer) != 0)
        {
            SlashBoostAction?.Invoke(BoostDirection, BoostPower);
        }
    }
}

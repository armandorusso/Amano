using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetsuoSlashAttack : MonoBehaviour
{
    [SerializeField] public LayerMask HitboxLayerMask;
    [SerializeField] public Vector2 BoostDirection;
    [SerializeField] public float BoostPower;

    private bool _isSlashing;
    public static Action<Vector2, float> SlashBoostAction;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (_isSlashing)
            return;
        
        var collidedGameObject = col.gameObject;

        if ((HitboxLayerMask.value & 1 << collidedGameObject.layer) != 0)
        {
            _isSlashing = true;
            SlashBoostAction?.Invoke(BoostDirection, BoostPower);
            Invoke(nameof(SetIsSlashingFalse), 0.12f);
        }
    }

    private void SetIsSlashingFalse()
    {
        _isSlashing = false;
        CancelInvoke(nameof(SetIsSlashingFalse));
    }
}

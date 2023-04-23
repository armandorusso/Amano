using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    public static Action<bool> TetsuoDeathZoneAction;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == 6)
        {
            TetsuoDisableMovement.Instance.ResetVelocity();
            TetsuoDeathZoneAction?.Invoke(false);
        }
    }
}

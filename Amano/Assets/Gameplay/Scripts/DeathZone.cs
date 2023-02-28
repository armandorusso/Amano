using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    public class TetsuoFallDeathEventArgs : EventArgs
    {
        public bool isMovementEnabled { get; set; }
    }

    public TetsuoFallDeathEventArgs tetsuoFallDeathEventArgs;
    public static event EventHandler<TetsuoFallDeathEventArgs> tetsuoFallDeathEvent;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == 6)
        {
            tetsuoFallDeathEventArgs = new TetsuoFallDeathEventArgs
            {
                isMovementEnabled = false
            };
            tetsuoFallDeathEvent?.Invoke(this, tetsuoFallDeathEventArgs);
        }
    }
}

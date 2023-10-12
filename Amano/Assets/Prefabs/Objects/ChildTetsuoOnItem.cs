using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildTetsuoOnItem : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (transform.parent.TryGetComponent(out MovingPlatform platform) &&
            col.gameObject.CompareTag("Player"))
        {
            col.transform.parent = platform.transform;
        }
    }

    private void OnCollisionExit(Collision col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            col.transform.parent = null;
        }
    }
}

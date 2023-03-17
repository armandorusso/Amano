using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSetter : MonoBehaviour
{
    [SerializeField] private LayerMask m_layerMask;
    public void OnRoomEntered()
    {
        GameManager.Instance.CurrentSpawnPoint = gameObject;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(m_layerMask == (m_layerMask | (1 << col.gameObject.layer)))
            OnRoomEntered();
    }
}

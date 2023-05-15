using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldSamuraiAnimationController : MonoBehaviour
{
    private Animator _animator;

    void Start()
    {
        _animator = GetComponent<Animator>();
        EnemyPatrolShieldState.IsWalkingAction += OnWalkingEvent;
    }

    private void OnWalkingEvent(GameObject sender, bool isWalking)
    {
        if (sender == gameObject)
        {
            _animator.SetBool("isWalking", isWalking);
        }
    }

    public void OnDestroy()
    {
        EnemyPatrolShieldState.IsWalkingAction -= OnWalkingEvent;
    }
}

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
        EnemySlashState.slashingEvent += OnSlashEvent;
    }

    private void Update()
    {

    }

    private void OnSlashEvent(object sender, EnemySlashState.SlashingEventAnimationArgs e)
    {
        _animator.SetBool("isSlashing", e.isSlashing);
    }
}

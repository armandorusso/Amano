using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySlashState : IAmanoState
{
    private ShieldSamuraiData _enemyData;
    private Transform _tetsuoTransform;
    private AmanoTimer _timer;
    private AmanoTimer _cooldownTimer;
    private bool _isInCooldown;
    private bool isSlashing;
    
    public class SlashingEventAnimationArgs : EventArgs
    {
        public bool isSlashing { get; set; }
    }
    public static event EventHandler<SlashingEventAnimationArgs> slashingEvent;
    private SlashingEventAnimationArgs slashingEventArgs;
    
    public void EnterState(AmanoStateMachine stateMachine)
    {
        _enemyData = stateMachine.GetComponent<ShieldSamuraiData>();
        _tetsuoTransform = _enemyData.TetsuoPosition;
        _timer = _enemyData.Timer;
        _cooldownTimer = _enemyData.CooldownTimer;
        slashingEventArgs = new SlashingEventAnimationArgs
        {
            isSlashing = false
        };
        
        ActivateSlash();
    }

    public void UpdateState(AmanoStateMachine stateMachine)
    {
        slashingEventArgs.isSlashing = isSlashing;
        if (Vector2.Distance(_tetsuoTransform.position, _enemyData.transform.position) >= 1f)
        {
            stateMachine.SwitchState("EnemyPatrolShieldState");
        }
        else
        {
            if (_timer.IsTimerDone())
            {
                SlashCooldown();
            }
            
            if (!_timer.IsTimerInProgress() && !_isInCooldown)
            {
                ActivateSlash();
            }
        }
    }

    private void ActivateSlash()
    {
        Debug.Log("Slashing!");
        _timer.StartTimer(1f);
        isSlashing = true;
        slashingEvent.Invoke(this, slashingEventArgs);
    }

    private void SlashCooldown()
    {
        if (_cooldownTimer.IsTimerDone() && !isSlashing)
        {
            ActivateSlash();
            _isInCooldown = false;
        }
        
        if (!_cooldownTimer.IsTimerInProgress() && !_timer.IsTimerInProgress())
        {
            Debug.Log("Cooldown");
            isSlashing = false;
            slashingEvent.Invoke(this, slashingEventArgs);
            _isInCooldown = true;
            _cooldownTimer.StartTimer(2.5f);
        }
    }

    public void ExitState(AmanoStateMachine stateMachine)
    {
        _timer.ResetTimer();
        slashingEventArgs.isSlashing = false;
        slashingEvent.Invoke(this, slashingEventArgs);
        _cooldownTimer.ResetTimer();
    }
}

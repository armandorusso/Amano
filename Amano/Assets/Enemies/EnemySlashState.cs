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
    public bool isSlashing;
    public void EnterState(AmanoStateMachine stateMachine)
    {
        _enemyData = stateMachine.GetComponent<ShieldSamuraiData>();
        _tetsuoTransform = _enemyData.TetsuoPosition;
        _timer = _enemyData.Timer;
        _cooldownTimer = _enemyData.CooldownTimer;
        ActivateSlash();
    }

    public void UpdateState(AmanoStateMachine stateMachine)
    {
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
            _isInCooldown = true;
            _cooldownTimer.StartTimer(2.5f);
        }
    }

    public void ExitState(AmanoStateMachine stateMachine)
    {
        _timer.ResetTimer();
        _cooldownTimer.ResetTimer();
    }
}

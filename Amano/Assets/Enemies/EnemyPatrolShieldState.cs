using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EnemyPatrolShieldState : IAmanoState
{
    private ShieldSamuraiData _enemyData;
    private Transform _tetsuoPosition;
    private int _direction;
    private Vector2 _playerDirection;
    public bool _isFacingRight;
    private AmanoTimer _timer;

    public void EnterState(AmanoStateMachine stateMachine)
    {
        _enemyData = stateMachine.GetComponent<ShieldSamuraiData>();
        _tetsuoPosition = _enemyData.TetsuoPosition;
        _timer = _enemyData.Timer;
        _timer.StartTimer(2f);
        _isFacingRight = _enemyData.transform.localScale.x == 1;
    }

    public void UpdateState(AmanoStateMachine stateMachine)
    {
        LineOfSight(stateMachine);
        Patrol();
    }

    public void ExitState(AmanoStateMachine stateMachine)
    {
        _timer.ResetTimer();
    }

    private void LineOfSight(AmanoStateMachine stateMachine)
    {
        var enemyPosition = _enemyData.transform.position;
        _playerDirection = _tetsuoPosition.position - enemyPosition;
        Debug.Log(_playerDirection.x);
        var hit = Physics2D.Raycast(enemyPosition, _playerDirection, _enemyData.LineOfSightDistance,
            _enemyData.RayCastLayers);
        Debug.DrawRay(enemyPosition, _playerDirection.normalized, Color.magenta);
        
        if (hit.distance <=
            _enemyData.LineOfSightDistance
            && hit.distance >= 1f)
        {
            // stateMachine.SwitchState("EnemySlashState");
        }
    }

    private void Patrol()
    {
        if (_timer.IsTimerInProgress())
        {
            _enemyData.Rb.velocity = new Vector2(_direction * _enemyData.EnemyParameters.Speed, _enemyData.Rb.velocity.y);
            
            // If its facing right && its moving in the negative direction, flip the sprite. Same thing for the opposite condition
            switch (_isFacingRight)
            {
                case true when _playerDirection.x < 0f:
                case false when _playerDirection.x > 0f:
                    Flip();
                    break;
            }

            if (CheckIfAtEdge())
                _direction *= -1;
        }
        else if (_timer.IsTimerDone())
        {
            _direction = Random.Range(-1, 2);
            _timer.StartTimer(2f);
        }
    }
    
    private bool CheckIfAtEdge()
    {
        Debug.DrawRay(_enemyData.GroundCheck.transform.position, Vector2.down * 1f, Color.green);
        Debug.DrawRay(_enemyData.BackGroundCheck.position, Vector2.down * 1f, Color.green);
        return !Physics2D.Raycast(_enemyData.GroundCheck.transform.position, Vector2.down,1f,_enemyData.GroundLayer)
            || !Physics2D.Raycast(_enemyData.BackGroundCheck.transform.position, Vector2.down,1f,_enemyData.GroundLayer);
    }

    private void Flip()
    {
        _isFacingRight = !_isFacingRight;
        Vector3 localScale = _enemyData.transform.localScale;
        localScale.x *= -1;
        _enemyData.transform.localScale = localScale;
    }
}

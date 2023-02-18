using System;
using System.Collections;
using Unity.VisualScripting;
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
        var tetsuoPosition = _tetsuoPosition.position;
        _playerDirection = tetsuoPosition - enemyPosition;
        Debug.Log(_playerDirection);
        var hit = Physics2D.Raycast(enemyPosition, _playerDirection, _enemyData.LineOfSightDistance,
            _enemyData.RayCastLayers);
        Debug.DrawRay(enemyPosition, _playerDirection.normalized, Color.magenta);
        
        if (Vector2.Distance(enemyPosition, tetsuoPosition) <= 1f)
        {
            stateMachine.SwitchState("EnemySlashState");
        }
    }

    private void Patrol()
    {
        if (_timer.IsTimerInProgress())
        {
            _enemyData.Rb.velocity = new Vector2(_direction * _enemyData.EnemyParameters.Speed, _enemyData.Rb.velocity.y);

            var localScale = _direction < 0 ? -1 : 1;
            FlipGroundCheck(localScale);
            
            // If its facing right && its moving in the negative direction, flip the sprite. Same thing for the opposite condition
            switch (_isFacingRight)
            {
                case true when _playerDirection.x < 0f:
                case false when _playerDirection.x > 0f:
                    Flip();
                    break;
            }

            if (CheckIfAtEdge())
            {
                _direction *= -1;
            }
        }
        else if (_timer.IsTimerDone())
        {
            _direction = Random.Range(-1, 2);
            _timer.StartTimer(2f);
        }
    }

    private void FlipGroundCheck(int position)
    {
        var transformLocalScale = _enemyData.GroundCheckDirectionModifier.transform.localScale;
        transformLocalScale.x = position;
        _enemyData.GroundCheckDirectionModifier.transform.localScale = transformLocalScale;
    }

    private bool CheckIfAtEdge()
    {
        Debug.DrawRay(_enemyData.GroundCheck.transform.position, Vector2.down * 1f, Color.green);
        // Debug.DrawRay(_enemyData.BackGroundCheck.position, Vector2.down * 1f, Color.blue);
        return !Physics2D.Raycast(_enemyData.GroundCheck.transform.position, Vector2.down, 1f, _enemyData.GroundLayer);
    }

    private void Flip()
    {
        _isFacingRight = !_isFacingRight;
        /*var transformLocalScale = _enemyData.transform.localScale;
        Vector3 localScale = transformLocalScale;
        localScale.x *= -1;
        _enemyData.transform.localScale = localScale;*/
        _enemyData.Sprite.flipX = !_isFacingRight;
    }
}

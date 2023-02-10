using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EnemyPatrolState : IAmanoState
{
    private NormalSamuraiData _enemyData;
    private int _direction;
    public bool _isFacingRight = true;
    private AmanoTimer _timer;

    public void EnterState(AmanoStateMachine stateMachine)
    {
        _enemyData = stateMachine.GetComponent<NormalSamuraiData>();
        _timer = _enemyData.Timer;
        _timer.StartTimer(2f);
    }

    public void UpdateState(AmanoStateMachine stateMachine)
    {
        Debug.Log("Patrol");
        if (_timer.IsTimerInProgress())
        {
            _enemyData.Rb.velocity = new Vector2(_direction * _enemyData.NormalSamuraiParameters.Speed, _enemyData.Rb.velocity.y);
            
            // If its facing right && its moving in the negative direction, flip the sprite. Same thing for the opposite condition
            switch (_isFacingRight)
            {
                case true when _direction < 0f:
                case false when _direction > 0f:
                    Flip();
                    break;
            }
            
            if(CheckIfAtEdge())
                _direction *= -1;
        }
        else if (_timer.IsTimerDone())
        {
            _direction = Random.Range(-1, 2);
            _timer.StartTimer(2f);
        }

        LineOfSight(stateMachine);
    }

    public void ExitState(AmanoStateMachine stateMachine)
    {
        
    }

    private void LineOfSight(AmanoStateMachine stateMachine)
    {
        var enemyPosition = _enemyData.gameObject.transform.position;
        var tetsuoPosition = _enemyData.TetsuoPosition.position;

        var direction = tetsuoPosition - enemyPosition; // first variable of the equation is your destination of the raycast. where do you want it hit?
        var normalizedDirection = direction.normalized;

        var hit = Physics2D.Raycast(enemyPosition, normalizedDirection, _enemyData.LineOfSightDistance, _enemyData.RayCastLayers);
        Debug.DrawRay(enemyPosition, normalizedDirection, Color.white);

        if (hit && hit.collider.CompareTag("Player"))
        {
            Debug.Log("Sees player!");
            stateMachine.SwitchState("EnemyShootState");
        }
    }

    private bool CheckIfAtEdge()
    {
        Debug.DrawRay(_enemyData.GroundCheck.transform.position, Vector2.down * 1f, Color.green);
        return !Physics2D.Raycast(_enemyData.GroundCheck.transform.position, Vector2.down,1f,_enemyData.GroundLayer);
    }

    private void Flip()
    {
        _isFacingRight = !_isFacingRight;
        Vector3 localScale = _enemyData.transform.localScale;
        localScale.x *= -1;
        _enemyData.transform.localScale = localScale;
    }
}
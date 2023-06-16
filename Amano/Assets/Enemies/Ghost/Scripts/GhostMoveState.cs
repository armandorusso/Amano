using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostMoveState : IAmanoState
{
    public GhostEnemyData _enemyData;
    public AmanoTimer _timer;

    private float startTime;
    private int _direction;
    
    public void EnterState(AmanoStateMachine stateMachine)
    {
        _enemyData = stateMachine.GetComponent<GhostEnemyData>();
        _timer = stateMachine.GetComponent<AmanoTimer>();
        startTime = Time.time;
        _direction = 1;
        _timer.StartTimer(2f);
    }

    public void UpdateState(AmanoStateMachine stateMachine)
    {
        if (_timer.IsTimerInProgress())
        {
            Vector3 newPos = stateMachine.transform.position;
            float offsetY =
                _enemyData.Amplitude *
                Mathf.Sin((Time.time - stateMachine.transform.position.y) * _enemyData.HeightSpeed); // Multiplying by the amplitude increases the height of the sine wave.
            // Multiplying by the height speed increases the speed at which the sine wave moves

            float offsetX =
                _enemyData.Amplitude *
                Mathf.Cos((Time.time - stateMachine.transform.position.x) * _enemyData.HeightSpeed);

            newPos.y = offsetY;
            newPos.x = offsetX;

            stateMachine.transform.position = newPos;
        }

        if (_timer.IsTimerDone())
        {
            _direction *= -1;
            _timer.StartTimer(2f);
        }
    }

    public void ExitState(AmanoStateMachine stateMachine)
    {
        throw new System.NotImplementedException();
    }
}

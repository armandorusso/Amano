using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RandomEnemyMoveState : IAmanoState
{
    public BirdEnemyData _enemyData;
    public AmanoTimer _timer;

    private float startTime;
    private int _direction;
    
    public void EnterState(AmanoStateMachine stateMachine)
    {
        _enemyData = stateMachine.GetComponent<BirdEnemyData>();
        _timer = stateMachine.GetComponent<AmanoTimer>();
        startTime = Time.time;
        _direction = 1;
        _timer.StartTimer(2f);
    }

    public void UpdateState(AmanoStateMachine stateMachine)
    {
        if (_timer.IsTimerInProgress())
        {
            float offset =
                _enemyData.Amplitude *
                Mathf.Sin((Time.time - startTime) * _enemyData.HeightSpeed); // Multiplying by the amplitude increases the height of the sine wave.
                                                                             // Multiplying by the height speed increases the speed at which the sine wave moves

            Vector3 newPos = stateMachine.transform.position;
            newPos.y = offset; // Set the y coordinate to be calculated sine function, since thats what sets the varied height value
            newPos.x += _enemyData.EnemyParameters.Speed *
                        _direction * Time.deltaTime; // To make the object move in wave motion, move the object in the x direction to simulate the wave

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
    }
}

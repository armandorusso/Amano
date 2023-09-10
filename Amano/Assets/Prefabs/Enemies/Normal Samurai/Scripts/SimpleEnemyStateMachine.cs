using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemyStateMachine : AmanoStateMachine
{
    protected override void Start()
    {
        _states = new HashSet<IAmanoState>(10);
        _defaultState = new EnemyPatrolState();
        _states.Add(_defaultState);
        _states.Add(new EnemyShootState());
        base.Start();
    }
}

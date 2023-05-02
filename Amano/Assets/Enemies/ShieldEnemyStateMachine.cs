using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldEnemyStateMachine : AmanoStateMachine
{
    protected override void Start()
    {
        _states = new HashSet<IAmanoState>(10);
        _defaultState = new EnemyPatrolShieldState();
        _states.Add(_defaultState);
        _states.Add(new EnemySlashState());
        base.Start();
    }
}

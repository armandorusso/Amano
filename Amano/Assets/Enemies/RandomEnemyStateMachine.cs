using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEnemyStateMachine : AmanoStateMachine
{
    // Start is called before the first frame update
    protected override void Start()
    {
        _states = new HashSet<IAmanoState>(10);
        _defaultState = new RandomEnemyMoveState();
        _states.Add(_defaultState);
        base.Start();
    }
}

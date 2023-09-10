using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostStateMachine : AmanoStateMachine
{
    protected override void Start()
    {
        _states = new HashSet<IAmanoState>(10);
        _defaultState = new GhostMoveState();
        _states.Add(_defaultState);
        base.Start();
    }
}

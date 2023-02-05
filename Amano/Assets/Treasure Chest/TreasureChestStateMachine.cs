using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChestStateMachine : AmanoStateMachine
{
    protected override void Start()
    {
        var closedChestInstance = new ClosedTreasureChestState();
        _states = new HashSet<IAmanoState>(10);
        _states.Add(new OpenedTreasureChestState());
        _states.Add(closedChestInstance);
        _currentState = closedChestInstance;
        base.Start();
    }

}

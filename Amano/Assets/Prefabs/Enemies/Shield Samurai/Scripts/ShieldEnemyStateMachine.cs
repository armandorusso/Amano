using System.Collections.Generic;

public class ShieldEnemyStateMachine : AmanoStateMachine
{
    protected override void Start()
    {
        _states = new HashSet<IAmanoState>(10);
        _defaultState = new EnemyPatrolShieldState();
        _states.Add(_defaultState);
        base.Start();
    }
}

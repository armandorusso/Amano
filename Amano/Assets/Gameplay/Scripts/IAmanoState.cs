using UnityEngine;

public interface IAmanoState
{
    public void EnterState(AmanoStateMachine stateMachine);
    public void UpdateState(AmanoStateMachine stateMachine);
    public void ExitState(AmanoStateMachine stateMachine);
}

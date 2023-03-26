using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenedTreasureChestState : IAmanoState
{
    private AmanoTimer _timer;

    public void EnterState(AmanoStateMachine stateMachine)
    {
        Debug.Log("Entered Opened Treasure Chest state");
        stateMachine.gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
        _timer = stateMachine.gameObject.GetComponentInChildren<AmanoTimer>();
        _timer.StartTimer(2f);
    }

    public void UpdateState(AmanoStateMachine stateMachine)
    {
        Debug.Log("Update Opened Treasure Chest state");

        if (_timer.IsTimerDone())
        {
            stateMachine.SwitchState("ClosedTreasureChestState");
        }
    }

    public void ExitState(AmanoStateMachine stateMachine)
    {
        Debug.Log("Exit Opened Treasure Chest state");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosedTreasureChestState : IAmanoState
{
    public void EnterState(AmanoStateMachine stateMachine)
    {
        Debug.Log("Entered Closed Treasure Chest state");
        stateMachine.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
    }

    public void UpdateState(AmanoStateMachine stateMachine)
    {
        Debug.Log("Update Closed Treasure Chest state");
        
        if (Input.GetKeyDown(KeyCode.F))
        {
            stateMachine.SwitchState("OpenedTreasureChestState");
        }
    }

    public void ExitState(AmanoStateMachine stateMachine)
    {
        Debug.Log("Exit Closed Treasure Chest state");
    }
}

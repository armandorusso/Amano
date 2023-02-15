using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShieldState : IAmanoState
{
    private EnemyData _enemyData;
    private Transform _tetsuoPosition;
    private int _direction;
    public bool _isFacingRight;
    private AmanoTimer _timer;
    
    public void EnterState(AmanoStateMachine stateMachine)
    {
        _enemyData = stateMachine.GetComponent<ShieldSamuraiData>();
        _tetsuoPosition = _enemyData.TetsuoPosition;
        _timer = _enemyData.Timer;
    }

    public void UpdateState(AmanoStateMachine stateMachine)
    {
        var enemyPosition = _enemyData.transform.position;
        var rayDirection = _tetsuoPosition.position - enemyPosition;
        var hit = Physics2D.Raycast(enemyPosition, rayDirection, _enemyData.LineOfSightDistance,
            _enemyData.RayCastLayers);
        Debug.DrawRay(enemyPosition, rayDirection.normalized, Color.magenta);
        
        if (hit.distance <=
            _enemyData.LineOfSightDistance
            && hit.distance >= 1f)
        {
            AdjustShieldPosition(rayDirection);
        }
        else
        {
            // stateMachine.SwitchState("EnemySlashState");
        }
    }

    private void AdjustShieldPosition(Vector3 direction)
    {
        float zRotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        _enemyData.EnemyParameters.ItemUsing.transform.rotation = Quaternion.Euler(0, 0, zRotation);
    }

    public void ExitState(AmanoStateMachine stateMachine)
    {
        throw new System.NotImplementedException();
    }
}

using System;
using System.Collections;
using UnityEngine;

public class EnemyShootState : IAmanoState
{
    private NormalSamuraiData _enemyData;
    private AmanoTimer _timer;
    private int _direction;
    
    public static Action<GameObject, bool> IsShootingAction;

    public void EnterState(AmanoStateMachine stateMachine)
    {
        Debug.Log("Entered Shoot State");
        _enemyData = stateMachine.GetComponent<NormalSamuraiData>();
        _timer = _enemyData.Timer;
        _timer.StartTimer(1f);
    }

    public void UpdateState(AmanoStateMachine stateMachine)
    {
        var enemyPosition = stateMachine.transform.position;
        if (Vector2.Distance(_enemyData.TetsuoPosition.position, enemyPosition) < _enemyData.LineOfSightDistance && _timer.IsTimerDone())
        {
            IsShootingAction?.Invoke(stateMachine.gameObject, true);

            ShootWeapon(enemyPosition, stateMachine);
        }
        else if(Vector2.Distance(_enemyData.TetsuoPosition.position, enemyPosition) > _enemyData.LineOfSightDistance)
        {
            stateMachine.SwitchState("EnemyPatrolState");
        }
        
        /*else if (Vector2.Distance(_enemyData.TetsuoPosition.position, enemyPosition) <= 1f)
        {
            stateMachine.SwitchState("EnemySlashState");
        }*/
    }

    private void ShootWeapon(Vector3 enemyPosition, AmanoStateMachine stateMachine)
    {
        var directionToShootProjectile = FaceInDirectionOfTetsuo(enemyPosition, out _);
        Debug.Log(directionToShootProjectile);
        var projectileInScene = GameObject.Instantiate(_enemyData.EnemyParameters.ItemUsing,
            _enemyData.ThrowPosition.position, Quaternion.identity);
        var rbOfProjectile = projectileInScene.GetComponent<Rigidbody2D>();

        rbOfProjectile.velocity = new Vector2(directionToShootProjectile.x, directionToShootProjectile.y) * _enemyData.ShootForce;
        stateMachine.StartCoroutine(SetThrowingAnimationToFalse(stateMachine));
        _timer.StartTimer(2f);
    }

    private IEnumerator SetThrowingAnimationToFalse(AmanoStateMachine stateMachine)
    {
        yield return new WaitForSeconds(0.8f);
        IsShootingAction?.Invoke(stateMachine.gameObject, false);
    }

    public void ExitState(AmanoStateMachine stateMachine)
    {
        IsShootingAction?.Invoke(stateMachine.gameObject, false);
    }

    private Vector3 FaceInDirectionOfTetsuo(Vector3 enemyPosition, out Vector3 enemyFaceDirection)
    {
        var directionToShootProjectile = (_enemyData.TetsuoPosition.position - enemyPosition).normalized;
        enemyFaceDirection = _enemyData.gameObject.transform.localScale;
        enemyFaceDirection.x = directionToShootProjectile.x < 0 ? -1 : 1;
        
        _enemyData.gameObject.transform.localScale = enemyFaceDirection;
        return directionToShootProjectile;
    }
}

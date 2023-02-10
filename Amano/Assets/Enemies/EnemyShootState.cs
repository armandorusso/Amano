using UnityEngine;

public class EnemyShootState : IAmanoState
{
    private NormalSamuraiData _enemyData;
    private AmanoTimer _timer;
    private int _direction;
    
    public void EnterState(AmanoStateMachine stateMachine)
    {
        Debug.Log("Entered Shoot State");
        _enemyData = stateMachine.GetComponent<NormalSamuraiData>();
        _timer = _enemyData.Timer;
    }

    public void UpdateState(AmanoStateMachine stateMachine)
    {
        var enemyPosition = stateMachine.transform.position;
        if (Vector2.Distance(_enemyData.TetsuoPosition.position, enemyPosition) < 2f)
        {
            var projectileInScene = ObjectPool.ObjectPoolInstance.GetFirstPooledObject();
            var rbOfProjectile = projectileInScene.GetComponent<Rigidbody2D>();
            var directionToShootProjectile = (_enemyData.TetsuoPosition.position - enemyPosition).normalized;

            rbOfProjectile.velocity = new Vector2(directionToShootProjectile.x, directionToShootProjectile.y) * 2f;
            _timer.StartTimer(1f);
        }
        else
        {
            stateMachine.SwitchState("EnemyPatrolState");
        }
    }

    public void ExitState(AmanoStateMachine stateMachine)
    {
        
    }
}

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
        if (Vector2.Distance(_enemyData.TetsuoPosition.position, enemyPosition) < 2f && _timer.IsTimerDone())
        {
            var directionToShootProjectile = FaceInDirectionOfTetsuo(enemyPosition, out _);
            Debug.Log(directionToShootProjectile);
            Vector3 rotation = directionToShootProjectile - _enemyData.ThrowPosition.position;
            float zRotation = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
            _enemyData.ThrowPosition.rotation = Quaternion.Euler(0, 0, zRotation);
            var projectileInScene = GameObject.Instantiate(_enemyData.NormalSamuraiParameters.Projectile, _enemyData.ThrowPosition.position, Quaternion.identity);
            var rbOfProjectile = projectileInScene.GetComponent<Rigidbody2D>();

            rbOfProjectile.velocity = new Vector2(directionToShootProjectile.x, directionToShootProjectile.y) * 20f;
            _timer.StartTimer(4f);
        }
        else if(Vector2.Distance(_enemyData.TetsuoPosition.position, enemyPosition) > 2f)
        {
            stateMachine.SwitchState("EnemyPatrolState");
        }
    }

    public void ExitState(AmanoStateMachine stateMachine)
    {
        
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

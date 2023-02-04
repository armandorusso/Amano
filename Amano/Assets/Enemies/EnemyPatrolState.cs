using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyPatrolState : EnemyBaseState
{
    private Rigidbody2D _rb;
    private Collider2D _collider;

    private float _timer;
    

    public override void EnterState(EnemyStateManager enemy)
    {
        _rb = enemy.GetComponent<Rigidbody2D>();
        _collider = enemy.GetComponent<Collider2D>();
        _timer = 0f;
    }

    public override void UpdateState(EnemyStateManager enemy)
    {
        Move(enemy);
        LineOfSight(enemy);
    }

    public override void OnCollisionEnter(EnemyStateManager enemy, Collision2D collider)
    {
    }

    private void Move(EnemyStateManager enemy)
    {
        if (_timer >= 2f)
        {
            var direction = Random.Range(-1, 2);
            _rb.velocity = new Vector2(direction * enemy.EnemyStats.Speed, _rb.velocity.y);
            _timer = 0f;
        }

        _timer += Time.deltaTime;
    }
    

    private void LineOfSight(EnemyStateManager enemy)
    {
        Debug.DrawRay(enemy.transform.position, enemy.transform.forward, Color.red);
        var hit = Physics2D.Raycast(enemy.transform.position, enemy.transform.forward);

        if (hit != null && hit.collider.CompareTag("Player"))
        {
            enemy.SwitchState(enemy.ShootState);
        }
    }

    private void CheckIfGrounded()
    {
        Physics2D.OverlapBox(groundCheck.position, 0.1f, groundLayer);
    }
}

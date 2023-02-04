using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateManager : MonoBehaviour
{
    private EnemyBaseState currentState;
    public EnemyPatrolState PatrolState = new EnemyPatrolState();
    public EnemyShootState ShootState = new EnemyShootState();
    public EnemyDamagedState DamagedState = new EnemyDamagedState();
    public EnemyDeathState DeathState = new EnemyDeathState();

    [SerializeField] public NormalSamuraiScriptableObject EnemyStats;

    void Start()
    {
        // On start, call the patrol state. Enemy only attacks if it has line of sight
        currentState = PatrolState;
        
        currentState.EnterState(this);
    }

    void Update()
    {
        currentState.UpdateState(this); // Calls a state where it requires checks every frame (or movement every frame)
    }

    public void SwitchState(EnemyBaseState state)
    {
        // Switches the state of the enemy and calls enter state
        currentState = state;
        state.EnterState(this);
    }


    private void OnCollisionEnter2D(Collision2D col)
    {
        currentState.OnCollisionEnter(this, col);
    }
}

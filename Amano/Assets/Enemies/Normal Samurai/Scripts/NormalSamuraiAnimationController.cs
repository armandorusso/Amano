using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalSamuraiAnimationController : MonoBehaviour
{
    private Animator _animator;
    
    void Start()
    {
        _animator = GetComponent<Animator>();

        EnemyPatrolState.IsWalkingAction += OnWalkEvent;
        EnemyShootState.IsShootingAction += OnShootShurikenEvent;
    }

    public void OnWalkEvent(GameObject sender, bool isWalking)
    {
        if (sender == gameObject)
        {
            _animator.SetBool("isWalking", isWalking);
        }
    }

    public void OnShootShurikenEvent(GameObject sender, bool isShootingShuriken)
    {
        if (sender == gameObject)
        {
            _animator.SetBool("isShootingShuriken", isShootingShuriken);
        }
    }
    public void OnSlashEvent(GameObject sender, bool isSlashing)
    {
        if (sender == gameObject)
        {
            _animator.SetBool("isSlashing", isSlashing);
        }
    }
    
    public void OnDestroy()
    {
        EnemyPatrolState.IsWalkingAction -= OnWalkEvent;
        EnemyShootState.IsShootingAction -= OnShootShurikenEvent;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatformSteppedOnState : IAmanoState
{
    private SpriteRenderer _fallingPlatformSprite;
    private Rigidbody2D _rb;
    private BoxCollider2D _collider;
    private bool _steppedOn;
    private bool _isPlatformBreaking;
    private bool _isFalling;
    
    public void EnterState(AmanoStateMachine stateMachine)
    {
        throw new System.NotImplementedException();
    }

    public void UpdateState(AmanoStateMachine stateMachine)
    {
        throw new System.NotImplementedException();
    }

    public void ExitState(AmanoStateMachine stateMachine)
    {
        throw new System.NotImplementedException();
    }
}

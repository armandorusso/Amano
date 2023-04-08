using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatformStateMachine : AmanoStateMachine
{
    [SerializeField] public float PlatformFallSpeed;
    [SerializeField] public float DelayBeforePlatformFalls;
    // Start is called before the first frame update
    protected override void Start()
    {
        _states = new HashSet<IAmanoState>(10);
        _defaultState = new FallingPlatformSteppedOnState();
        _states.Add(_defaultState);
        _states.Add(new FallingPlatformRespawnState());
        base.Start();
    }

}

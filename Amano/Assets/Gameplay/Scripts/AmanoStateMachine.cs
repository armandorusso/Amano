using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class AmanoStateMachine : MonoBehaviour
{
    protected IAmanoState _currentState;
    protected IAmanoState _defaultState;
    protected HashSet<IAmanoState> _states;

    public static event EventHandler<OnStateChangedArgs> onStateChangedEvent;
    public class OnStateChangedArgs : EventArgs
    {
        public string previousState;
        public string newState;
    }

    protected virtual void Start()
    {
        _currentState = _defaultState;
        _currentState.EnterState(this);
    }

    protected void Update()
    {
        _currentState.UpdateState(this);
    }

    public void SwitchState(string stateName)
    {
        var previousState = _currentState;

        var requestedState = _states.First(x => x.GetType().Name.Equals(stateName));
        Assert.IsNotNull(requestedState);
        
        _currentState.ExitState(this);
        _currentState = requestedState;
        _currentState.EnterState(this);
        
        // Implement later
        /*onStateChangedEvent.Invoke(this, new OnStateChangedArgs
        {
            previousState = previousState.GetType().Name,
            newState = _currentState.GetType().Name
        });*/
    }
}

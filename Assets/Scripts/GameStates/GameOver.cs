using StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : IState
{
    private IState.StateEventDelegate _onStateEnter;
    public IState.StateEventDelegate OnStateEnter { set => _onStateEnter = value; }
    private IState.StateEventDelegate _onStateUpdate;
    public IState.StateEventDelegate OnStateUpdate { set => _onStateUpdate = value; }
    private IState.StateEventDelegate _onStateExit;
    public IState.StateEventDelegate OnStateExit { set => _onStateExit = value; }

    public void Enter()
    {
        _onStateEnter?.Invoke();
    }

    public void Exit()
    {
        _onStateExit?.Invoke();
    }

    public void Update()
    {
        _onStateUpdate?.Invoke();
    }
}

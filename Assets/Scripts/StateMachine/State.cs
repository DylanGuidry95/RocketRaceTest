using System;

namespace StateMachine
{
    public interface IState
    {
        delegate void StateEventDelegate();
                
        StateEventDelegate OnStateEnter { set; }
        StateEventDelegate OnStateUpdate{ set; }
        StateEventDelegate OnStateExit{ set; }

        void Enter();
        void Update();
        void Exit();
    }
}

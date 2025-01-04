using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using StateMachine.Parameters;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace StateMachine
{
    public class GGD_StateMachine
    {
        private IState _currentState;
        private List<Transition> _transitions;
        private List<Parameter> _parameters;
 
        public GGD_StateMachine()
        {
            _transitions = new List<Transition>();
            _parameters = new List<Parameter>();
        }

        public void StartMachine(IState startingState)
        {
            if (_transitions == null)
                throw new Exception("No transitions defined for this machine");
            _currentState = startingState;
            _currentState.Enter();
        }
        
        public void Update()
        {
            _currentState.Update();
            var nextState = CheckForTransition();
            if (nextState != _currentState)
            {
                _currentState.Exit();
                _currentState = nextState;
                _currentState.Enter();
            }

            foreach (var parameter in _parameters) 
            {
                if (parameter.GetType() == typeof(TriggerParameter))
                {
                    var triggerParameter = parameter as TriggerParameter;
                    if (triggerParameter == true)
                    {
                        triggerParameter.Value = false;
                    }
                }
            }
        }
        
        public Parameters.Parameter AddParameter(Parameters.Parameter parameter)
        {
            _parameters.Add(parameter);
            return parameter;
        }
        
        public void SetParameter(string name, object value)
        {
            GetParameterByName(name).Value = value;
        }
        
        public void SetTrigger(string name)
        {
            var param = GetParameterByName(name);
            if(param.GetType() == typeof(TriggerParameter))
            {
                var triggerParameter = param as TriggerParameter;
                triggerParameter.Trigger();
            }
        }

        public object GetParameterValue(string name)
        {
            foreach (var parameter in _parameters)
            {
                if (parameter.Name == name)
                    return parameter.Value;
            }
            return null;
        }

        private Parameters.Parameter GetParameterByName(string name)
        {
            foreach(var parameter in _parameters)
            {
                if (parameter.Name == name)
                    return parameter;
            }
            return null;
        }
        
        public Transition DefineTransition(IState entry, IState exit, Condition[] conditions = null, int weight = 0)
        {
            _transitions ??= new List<Transition>();
            var transitionToAdd = new Transition(entry, exit, conditions, weight); 
            _transitions.Add(transitionToAdd);
            return transitionToAdd;
        }
        private IState CheckForTransition()
        {
            var transitionsFromCurrent = AllTransitionsFrom(_currentState);
            Transition bestTransitionOption = null;
            var nextState = _currentState;
            foreach (var transition in transitionsFromCurrent)
            {
                var endState = transition.MeetsTransitionRequirements();
                if (endState == null) continue;
                if (bestTransitionOption == null)
                {
                    bestTransitionOption = transition;
                    nextState = endState;
                }
                else
                {
                    if (transition.Weight <= bestTransitionOption.Weight) continue;
                    bestTransitionOption = transition;
                    nextState = endState;
                }
            }

            return nextState;

        }
        private List<Transition> AllTransitionsFrom(IState state)
        {
            var retValue = new List<Transition>();
            foreach (var transition in _transitions)
            {
                if(transition.EntryState == state)
                    retValue.Add(transition);
            }

            if (retValue.Count == 0)
                Debug.LogWarning($"No transitions defined from {state.GetType()}, is this intentional?");
            return retValue;
        }        
    }
}
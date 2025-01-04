using System.Collections.Generic;
using System.Linq;

namespace StateMachine
{
    public class Transition
    {
        private IState _entryState;
        public IState EntryState => _entryState;
        
        private IState _exitState;
        public IState ExitState => _exitState;

        private List<Condition> _conditionsForTransition;
        public int Weight { get; } = 0;

        public Transition(IState entry, IState exit,  Condition[] conditions = null, int weight = 0)
        {
            _entryState = entry;
            _exitState = exit;
            if (conditions != null) _conditionsForTransition = conditions.ToList();
            Weight = weight;
        }

        public void AddCondition(Condition condition)
        {
            _conditionsForTransition ??= new List<Condition>();
            _conditionsForTransition.Add(condition);
        }

        public IState MeetsTransitionRequirements()
        {
            if (_conditionsForTransition == null)
                return _exitState;
            
            return _conditionsForTransition.Any(condition => !condition.Evaluate()) ? null : _exitState;
        }        
    }
}
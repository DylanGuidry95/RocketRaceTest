using StateMachine.Parameters;

namespace StateMachine
{
    public class Condition
    {
        #region Fields
        private Parameter _leftHandSide;
        private object _rightHandSide;
        private ConditionOperations _operation;
        #endregion

        #region Properties
        #endregion

        #region Constructor
        public Condition(Parameter lhs, ConditionOperations operation, object rhs)
        {
            _leftHandSide = lhs;
            _operation = operation;
            _rightHandSide = rhs;
        }
        #endregion

        #region Methods
        public bool Evaluate()
        {
            switch(_operation)
            {
                case ConditionOperations.equals_to:
                    if (_leftHandSide is IntParameter)
                        return (_leftHandSide as IntParameter) == _rightHandSide;
                    else if (_leftHandSide is FloatParameter)
                        return (_leftHandSide as FloatParameter) == _rightHandSide;
                    else if (_leftHandSide is StringParameter)
                        return (_leftHandSide as StringParameter) == _rightHandSide;
                    else if (_leftHandSide is BooleanParameter)
                        return (_leftHandSide as BooleanParameter) == _rightHandSide;
                    else
                        break;
                case ConditionOperations.not_equals_to:
                    if (_leftHandSide is IntParameter)
                        return (_leftHandSide as IntParameter) != _rightHandSide;
                    else if (_leftHandSide is FloatParameter)
                        return (_leftHandSide as FloatParameter) != _rightHandSide;
                    else if (_leftHandSide is StringParameter)
                        return (_leftHandSide as StringParameter) != _rightHandSide;
                    else if (_leftHandSide is BooleanParameter)
                        return (_leftHandSide as BooleanParameter) != _rightHandSide;
                    else
                        break;
                case ConditionOperations.less_than:
                    if (_leftHandSide is IntParameter)
                        return (_leftHandSide as IntParameter) < _rightHandSide;
                    else if (_leftHandSide is FloatParameter)
                        return (_leftHandSide as FloatParameter) < _rightHandSide;
                    else
                        break;
                case ConditionOperations.less_than_or_equals_to:
                    if (_leftHandSide is IntParameter)
                        return (_leftHandSide as IntParameter) <= _rightHandSide;
                    else if (_leftHandSide is FloatParameter)
                        return (_leftHandSide as FloatParameter) <= _rightHandSide;
                    else
                        break;
                case ConditionOperations.greater_than:
                    if (_leftHandSide is IntParameter)
                        return (_leftHandSide as IntParameter) > _rightHandSide;
                    else if (_leftHandSide is FloatParameter)
                        return (_leftHandSide as FloatParameter) > _rightHandSide;
                    else
                        break;
                case ConditionOperations.greater_than_or_equals_to:
                    if (_leftHandSide is IntParameter)
                        return (_leftHandSide as IntParameter) > _rightHandSide;
                    else if (_leftHandSide is FloatParameter)
                        return (_leftHandSide as FloatParameter) >= _rightHandSide;
                    else
                        break;
            }
            return false;
        }
        #endregion
    }
}

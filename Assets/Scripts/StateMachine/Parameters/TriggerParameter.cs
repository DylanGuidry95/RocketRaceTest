using StateMachine.Parameters;

public class TriggerParameter : BooleanParameter
{
    public TriggerParameter(string name, bool initialValue = false) : base(name, initialValue)
    {
    }

    public void Trigger()
    {
        Value = true;
    }
}
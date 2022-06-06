namespace Ieg.Sequencer.Descriptors;

public class StateActionDescriptor : SequenceDescriptor
{
    public StateActionDescriptor(string state, Action action)
    {
        this.State  = state;
        this.Action = action;
    }

    public string State  { get; init; }
    public Action Action { get; init; }


    public override string ToString() => $"{State} (Action)";
    
    public bool ValidateState(string state) => state == State;
}
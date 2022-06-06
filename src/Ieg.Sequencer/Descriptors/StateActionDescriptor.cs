namespace Ieg.Sequencer.Descriptors;

public class StateActionDescriptor : SequenceDescriptor
{
    public StateActionDescriptor(string State, Action Action)
    {
        this.State = State;
        this.Action = Action;
    }

    public override string ToString() => $"{State} (Action)";

    
    public bool ValidateState(string state) => state == State;
    public string State { get; init; }
    public Action Action { get; init; }

    public void Deconstruct(out string State, out Action Action)
    {
        State = this.State;
        Action = this.Action;
    }
}
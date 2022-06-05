namespace Ieg.Sequencer.Descriptors;

public record StateActionDescriptor(string State, Action Action) : SequenceDescriptor
{
    public override string ToString() => $"{State} (Action)";

    
    public bool ValidateState(string state) => state == State;
}
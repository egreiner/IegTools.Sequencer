namespace Ieg.Sequencer.Descriptors;

public class StateActionDescriptor : SequenceDescriptor
{
    public StateActionDescriptor(string state, Action action)
    {
        State  = state;
        Action = action;
    }

    
    public string State  { get; }
    public Action Action { get; }


    public override string ToString() => $"{State} (Action)";
    
    public override bool ValidateAction(ISequence sequence) => State == sequence.CurrentState;

    public override void ExecuteAction(ISequence sequence)
    {
        Action?.Invoke();
    }
}
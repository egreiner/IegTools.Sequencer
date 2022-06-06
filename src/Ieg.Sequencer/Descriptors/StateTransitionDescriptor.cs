namespace Ieg.Sequencer.Descriptors;

public class StateTransitionDescriptor : SequenceDescriptor
{
    public StateTransitionDescriptor(string currentState, string nextState, Func<bool> constraint, Action action)
    {
        CurrentState = currentState;
        NextState    = nextState;
        Constraint   = constraint;
        Action       = action;
    }


    public string CurrentState   { get; }
    public string NextState      { get; }
    public Func<bool> Constraint { get; }
    public Action Action         { get; }

    
    public override string ToString() =>
        $"{CurrentState}->{NextState} (Transition)";

    
    public override bool ValidateAction(ISequence sequence) =>
        CurrentState == sequence.CurrentState && (Constraint?.Invoke() ?? true);

    
    public override void ExecuteAction(ISequence sequence)
    {
        sequence.SetState(NextState);
        Action?.Invoke();
    }
}
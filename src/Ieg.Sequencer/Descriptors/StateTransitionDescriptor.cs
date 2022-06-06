namespace Ieg.Sequencer.Descriptors;

public class StateTransitionDescriptor : SequenceDescriptor
{
    public StateTransitionDescriptor(string currentState, string nextState, Func<bool> constraint, Action action)
    {
        this.CurrentState = currentState;
        this.NextState    = nextState;
        this.Constraint   = constraint;
        this.Action       = action;
    }


    public string CurrentState { get; init; }
    public string NextState { get; init; }
    public Func<bool> Constraint { get; init; }
    public Action Action { get; init; }

    
    public override string ToString() =>
        $"{CurrentState}->{NextState} (Transition)";

    public bool ValidateTransition(string state) =>
        state == CurrentState && (Constraint?.Invoke() ?? true);
}
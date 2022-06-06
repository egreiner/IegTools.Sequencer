namespace Ieg.Sequencer.Descriptors;

public class StateTransitionDescriptor : SequenceDescriptor
{
    public StateTransitionDescriptor(string CurrentState, string NextState, Func<bool> Constraint, Action Action)
    {
        this.CurrentState = CurrentState;
        this.NextState = NextState;
        this.Constraint = Constraint;
        this.Action = Action;
    }

    public override string ToString() =>
        $"{CurrentState}->{NextState} (Transition)";

    public bool ValidateTransition(string state) =>
        state == CurrentState && (Constraint?.Invoke() ?? true);

    public string CurrentState { get; init; }
    public string NextState { get; init; }
    public Func<bool> Constraint { get; init; }
    public Action Action { get; init; }

    public void Deconstruct(out string CurrentState, out string NextState, out Func<bool> Constraint, out Action Action)
    {
        CurrentState = this.CurrentState;
        NextState = this.NextState;
        Constraint = this.Constraint;
        Action = this.Action;
    }
}
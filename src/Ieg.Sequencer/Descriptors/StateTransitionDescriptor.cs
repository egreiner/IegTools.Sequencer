namespace Ieg.Sequencer.Descriptors;

public record StateTransitionDescriptor(string CurrentState, string NextState, Func<bool> Constraint, Action Action) : SequenceDescriptor
{
    public override string ToString() =>
        $"{CurrentState}->{NextState} (Transition)";

    public bool ValidateTransition(string state) =>
        state == CurrentState && (Constraint?.Invoke() ?? true);
}
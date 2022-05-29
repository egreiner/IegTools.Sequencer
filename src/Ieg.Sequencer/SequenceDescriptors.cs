
namespace Ieg.Sequencer;

using System;

public abstract record SequenceDescriptor;

public record ForceStateDescriptor(string State, Func<bool> Constraint) : SequenceDescriptor
{
    public override string ToString() => $"{State} (Forced)";
}

public record StateActionDescriptor(string State, Action Action) : SequenceDescriptor
{
    public override string ToString() => $"{State} (Action)";
}

public record StateTransitionDescriptor(string CurrentState, string NextState, Func<bool> Constraint, Action Action) : SequenceDescriptor
{
    public override string ToString() =>
        $"{CurrentState}->{NextState} (Transition)";

    public bool ValidateTransition(string state) =>
        state == CurrentState && (Constraint?.Invoke() ?? true);
}
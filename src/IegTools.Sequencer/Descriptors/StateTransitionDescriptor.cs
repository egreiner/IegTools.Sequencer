﻿namespace IegTools.Sequencer.Descriptors;

/// <summary>
/// Transfers the sequence from the current state to the next state
/// if the constraint is met and invokes the specified action
/// </summary>
public class StateTransitionDescriptor : DescriptorBase, IHasFromState, IHasToState
{
    public StateTransitionDescriptor(string fromState, string toState, Func<bool> constraint, Action action)
    {
        FromState  = fromState;
        ToState    = toState;
        Constraint = constraint;
        Action     = action;
    }


    /// <summary>
    /// The state from which the transition should be made
    /// </summary>
    public string FromState   { get; }

    /// <summary>
    /// The state to which the transition should be made
    /// </summary>
    public string ToState      { get; }


    /// <summary>
    /// The action that will be invoked when the state transition will be executed
    /// </summary>
    public Action Action         { get; }

    
    public override string ToString() =>
        $"{FromState}->{ToState} (Transition)";


    /// <inheritdoc />
    public override bool IsRegisteredState(string state) =>
        state == ToState || state == FromState;

    /// <summary>
    /// Returns true if the sequence met the specified state and the constraint is fulfilled
    /// </summary>
    /// <param name="sequence">The sequence</param>
    public override bool ValidateAction(ISequence sequence) =>
        sequence.HasCurrentState(FromState) && IsConditionFulfilled(sequence);


    /// <summary>
    /// Invokes the action if the validation is fulfilled
    /// </summary>
    /// <param name="sequence">The sequence</param>
    public override void ExecuteAction(ISequence sequence)
    {
        sequence.SetState(ToState);

        if (sequence.ValidationOnly) return;
        Action?.Invoke();
    }
}
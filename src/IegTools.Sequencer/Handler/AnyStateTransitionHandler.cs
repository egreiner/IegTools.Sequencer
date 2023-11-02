﻿namespace IegTools.Sequencer.Handler;

using System.Linq;

/// <summary>
/// Transfers the sequence from the current state to the next state
/// when the condition is met
/// and invokes the specified action
/// </summary>
public class AnyStateTransitionHandler : HandlerBase, IHasToState
{
    /// <summary>
    /// Creates a new instance of the <see cref="AnyStateTransitionHandler"/>
    /// </summary>
    /// <param name="fromStates">The current sequence-states as precondition for any further actions</param>
    /// <param name="toState">The state the sequence will transition to when the condition is fulfilled</param>
    /// <param name="condition">The condition that must be fulfilled to execute the state-transition</param>
    /// <param name="action">The action that will be executed after the transition</param>
    public AnyStateTransitionHandler(string[] fromStates, string toState, Func<bool> condition, Action action)
    {
        FromStates = fromStates;
        ToState    = toState;
        Condition  = condition;
        Action     = action;
    }


    /// <summary>
    /// The state from which the transition should be made
    /// </summary>
    public string[] FromStates   { get; }

    /// <summary>
    /// The state to which the transition should be made
    /// </summary>
    public string ToState      { get; }

    
    public override string ToString() =>
        $"Any-StateTransition: [ {string.Join(", ", FromStates)} ]->{ToState}";


    /// <inheritdoc />
    public override bool IsRegisteredState(string state) =>
        state == ToState || FromStates.Contains(state);

    /// <summary>
    /// Returns true if the sequence met the specified state and the condition is fulfilled
    /// </summary>
    /// <param name="sequence">The sequence</param>
    public override bool IsConditionFulfilled(ISequence sequence) =>
        !sequence.HasCurrentState(ToState) && FromStates.Contains(sequence.CurrentState) && IsConditionFulfilled();


    /// <summary>
    /// Executes the specified action and transitions to the new state
    /// </summary>
    /// <param name="sequence">The sequence</param>
    public override void ExecuteAction(ISequence sequence)
    {
        sequence.SetState(ToState);
        Action?.Invoke();
    }
}
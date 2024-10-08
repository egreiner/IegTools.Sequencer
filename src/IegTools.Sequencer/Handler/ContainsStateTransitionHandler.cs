﻿namespace IegTools.Sequencer.Handler;

/// <summary>
/// Transfers the sequence from the current state to the next state
/// when the condition is met
/// and invokes the specified action
/// </summary>
public class ContainsStateTransitionHandler : HandlerBase, IHasToState
{
    /// <summary>
    /// Creates a new instance of the <see cref="ContainsStateTransitionHandler"/>
    /// </summary>
    /// <param name="fromStateContains">The current sequence-state must contain this string as precondition for any further actions</param>
    /// <param name="toState">The state the sequence will transition to when the condition is fulfilled</param>
    /// <param name="condition">The condition that must be fulfilled to execute the state-transition</param>
    /// <param name="action">The action that will be executed after the transition</param>
    /// <param name="description">The transition description (for debugging or just to describe what is it for)</param>
    public ContainsStateTransitionHandler(string fromStateContains, string toState, Func<bool> condition, Action action, string description = "")
        : base(condition, action, description)
    {
        Name              = "Contains-State Transition";
        FromStateContains = fromStateContains;
        ToState           = toState;
    }


    /// <summary>
    /// The state from which the transition should be made
    /// </summary>
    public string FromStateContains   { get; }

    /// <summary>
    /// The state to which the transition should be made
    /// </summary>
    public string ToState      { get; }

    
    /// <summary>
    /// Returns a string representation of the handler-state
    /// </summary>
    public override string ToString() =>
        $"Contains-State-Transition: *{FromStateContains}* -> {ToState}";


    /// <inheritdoc />
    public override bool IsRegisteredState(string state) =>
        state == ToState;

    /// <summary>
    /// Returns true if the sequence met the specified state and the condition is fulfilled
    /// </summary>
    public override bool ExecuteActionAllowed() =>
        !Sequence.HasCurrentState(ToState) &&
        Sequence.CurrentState.Contains(FromStateContains) &&
        TimeLockExpired &&
        ConditionSatisfied;


    /// <summary>
    /// Executes the specified action and transitions to the new state
    /// </summary>
    public override void ExecuteAction()
    {
        using var scope = Logger?.GetSequenceLoggerScope(this, "Execute Action");
        Logger?.LogDebug(Logger.EventId, "{Handler} -> from state {StateFrom} to state {StateTo}", Name, Sequence.CurrentState, ToState);

        SetState(ToState);
        TryInvokeAction();
    }
}
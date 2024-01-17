namespace IegTools.Sequencer.Handler;

using Microsoft.Extensions.Logging;

/// <summary>
/// Transfers the sequence from the current state to the next state
/// if the condition is met and invokes the specified action
/// </summary>
public class StateTransitionHandler : HandlerBase, IHasToState
{
    /// <summary>
    /// Creates a new instance of the <see cref="StateTransitionHandler"/>
    /// </summary>
    /// <param name="fromState">The current sequence-state as precondition for any further actions</param>
    /// <param name="toState">The state the sequence will transition to when the condition is fulfilled</param>
    /// <param name="condition">The condition that must be fulfilled to execute the state-transition</param>
    /// <param name="action">The action that will be executed after the transition</param>
    /// <param name="description">The transition description (for debugging or just to describe what is it for)</param>
    public StateTransitionHandler(string fromState, string toState, Func<bool> condition, Action action, string description = "")
        : base(condition, action, description)
    {
        Name      = "State Transition";
        FromState = fromState;
        ToState   = toState;
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
    /// Returns a string representation of the handler-state
    /// </summary>
    public override string ToString() =>
        $"State-Transition: {FromState} -> {ToState}";


    /// <inheritdoc />
    public override bool IsRegisteredState(string state) =>
        state.IsIn(FromState, ToState);

    /// <summary>
    /// Returns true if the sequence met the specified state and the condition is fulfilled
    /// </summary>
    /// <param name="sequence">The sequence</param>
    public override bool IsConditionFulfilled(ISequence sequence) =>
        sequence.HasCurrentState(FromState) &&
        TimeLockExpired &&
        ConditionSatisfied;


    /// <summary>
    /// Executes the specified action and transitions to the new state
    /// </summary>
    /// <param name="sequence">The sequence</param>
    public override void ExecuteAction(ISequence sequence)
    {
        using var scope = Logger?.GetSequenceLoggerScope(this, "Execute Action");
        Logger?.LogDebug(Logger.EventId, "{Handler} -> from state {StateFrom} to state {StateTo}", Name, FromState, ToState);

        SetState(ToState);
        TryInvokeAction();
    }
}
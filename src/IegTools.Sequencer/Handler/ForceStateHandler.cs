namespace IegTools.Sequencer.Handler;

/// <summary>
/// Forces the sequence to the specified state if the condition is fulfilled
/// </summary>
public class ForceStateHandler : HandlerBase, IHasToState
{
    /// <summary>
    /// Creates a new instance of the <see cref="AnyStateTransitionHandler"/>
    /// </summary>
    /// <param name="toState">The state the sequence will transition to when the condition is fulfilled</param>
    /// <param name="condition">The condition that must be fulfilled to execute the state-transition</param>
    /// <param name="action">The action that will be executed after the transition</param>
    public ForceStateHandler(string toState, Func<bool> condition, Action action)
    {
        ToState        = toState;
        Condition      = condition;
        ResumeSequence = false;
        Action         = action;
    }


    /// <summary>
    /// The state the sequence should be forced to
    /// </summary>
    public string ToState { get; }
    
    
    /// <summary>
    /// Returns a string representation of the handler-state
    /// </summary>
    public override string ToString() =>
        $"Force-State: {ToState}";


    /// <inheritdoc />
    public override bool IsRegisteredState(string state) =>
        state == ToState;

    /// <summary>
    /// This handler is not dependent on the current sequence state.
    /// It depends on the condition only.
    /// </summary>
    /// <param name="sequence">The sequence</param>
    public override bool IsConditionFulfilled(ISequence sequence) =>
        !sequence.HasCurrentState(ToState) && (Condition?.Invoke() ?? false);

    /// <summary>
    /// Transitions to the new state
    /// </summary>
    /// <param name="sequence">The sequence</param>
    public override void ExecuteAction(ISequence sequence) =>
        sequence.SetState(ToState);
}
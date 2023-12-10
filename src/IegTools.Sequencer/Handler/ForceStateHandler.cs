namespace IegTools.Sequencer.Handler;

using Microsoft.Extensions.Logging;

/// <summary>
/// Forces the sequence to the specified state if the condition is fulfilled
/// </summary>
public class ForceStateHandler : HandlerBase, IHasToState
{
    private bool _loggingDone;

    /// <summary>
    /// Creates a new instance of the <see cref="AnyStateTransitionHandler"/>
    /// </summary>
    /// <param name="toState">The state the sequence will transition to when the condition is fulfilled</param>
    /// <param name="condition">The condition that must be fulfilled to execute the state-transition</param>
    /// <param name="action">The action that will be executed after the transition</param>
    public ForceStateHandler(string toState, Func<bool> condition, Action action)
    {
        Name           = "Force State";
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
    public override bool IsConditionFulfilled(ISequence sequence)
    {
        var result = !sequence.HasCurrentState(ToState) && (Condition?.Invoke() ?? false);

        if (!result) _loggingDone = false;

        return result;
    }

    /// <summary>
    /// Transitions to the new state
    /// </summary>
    /// <param name="sequence">The sequence</param>
    public override void ExecuteAction(ISequence sequence)
    {
        if (!_loggingDone)
        {
            Logger?.Log(LogLevel.Debug, EventId, "{Method} - {Handler} Forced to {StateTo}", "Execute Action", Name, ToState);
            _loggingDone = true;
        }

        sequence.SetState(ToState);
    }
}
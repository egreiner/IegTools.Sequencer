namespace IegTools.Sequencer.Handler;

// TODO this can be built as simply two StateTransitionHandler

/// <summary>
/// Toggles the sequence between set-state and reset-state
/// if the condition for set or reset is met.
/// The set-condition is dominant, that means if the set-condition and reset-condition is met,
/// the sequence will be set to the set-state.
/// </summary>
public class ToggleStatesHandler : HandlerBase
{
    /// <summary>
    /// Creates a new instance of the <see cref="ToggleStatesHandler"/>
    /// </summary>
    /// <param name="fromState">The current sequence-state as precondition for any further actions</param>
    /// <param name="toState">The state the sequence will transition to when the condition is fulfilled</param>
    /// <param name="condition">The condition that must be fulfilled to execute the state-transition</param>
    /// <param name="action">The action that will be executed after the transition</param>
    /// <param name="description">The transition description (for debugging or just to describe what is it for)</param>
    public ToggleStatesHandler(
        string     resetState,           string     setState,
        Func<bool> dominantSetCondition, Func<bool> resetCondition,
        Action     setAction = null,     Action     resetAction = null,
        string description = "")
        : base(dominantSetCondition, setAction, description)
    {
        Name           = "Toggle States";
        ResetState     = resetState;
        SetState       = setState;
        ResetCondition = resetCondition;
        ResetAction    = resetAction;
    }


    public string ResetState { get; }
    public string SetState   { get; }

    public Func<bool> ResetCondition { get; }
    public Action     ResetAction    { get; }


    /// <summary>
    /// Returns a string representation of the handler-state
    /// </summary>
    public override string ToString() =>
        $"{Name}: {ResetState} -> {SetState}";


    /// <inheritdoc />
    public override bool IsRegisteredState(string state) =>
        state == ResetState || state == SetState;

    /// <summary>
    /// Returns true if the sequence met the specified state and the condition is fulfilled
    /// </summary>
    /// <param name="sequence">The sequence</param>
    public override bool IsConditionFulfilled(ISequence sequence) =>
        IsSetConditionFulfilled() || IsResetConditionFulfilled();

    /// <summary>
    /// Returns true if the handler set-condition is fulfilled
    /// </summary>
    private bool IsSetConditionFulfilled() =>
        Sequence.HasCurrentState(ResetState) && IsTimeOver && (Condition?.Invoke() ?? true);

    /// <summary>
    /// Returns true if the handler reset-condition is fulfilled
    /// </summary>
    private bool IsResetConditionFulfilled() =>
        Sequence.HasCurrentState(SetState) && IsTimeOver && (ResetCondition?.Invoke() ?? true);


    /// <summary>
    /// Executes the specified action and transitions to the new state
    /// </summary>
    /// <param name="sequence">The sequence</param>
    public override void ExecuteAction(ISequence sequence)
    {
        using var scope = Logger?.GetSequenceLoggerScope(this, "Execute Action");

        if (IsSetConditionFulfilled())
        {
            Logger?.LogDebug(Logger.EventId, "{Handler} -> set state {SetState}", Name, SetState);

            SetState(SetState);
            TryInvokeAction();
            return;
        }

        var lockReset = Condition?.Invoke() ?? false;
        if (!lockReset && IsResetConditionFulfilled())
        {
            Logger?.LogDebug(Logger.EventId, "{Handler} -> reset to state {ResetState}", Name, ResetState);

            SetState(ResetState);
            TryInvokeResetAction();
        }


        // Logger?.LogDebug(Logger.EventId, "{Handler} -> from state {ResetState} to state {SetState}", Name, ResetState, SetState);
    }

    /// <summary>
    /// Invokes the specified action,
    /// sets the last execution time
    /// and invokes the OnStateChangedAction if the state has changed
    /// </summary>
    private void TryInvokeResetAction()
    {
        try
        {
            ResetAction?.Invoke();

            // if (Action is not null)
                // LastExecutedAt = DateTime.Now;
        }
        catch (Exception e)
        {
            Logger?.LogError(Logger.EventId, e, "Try to invoke action failed");
        }
    }
}
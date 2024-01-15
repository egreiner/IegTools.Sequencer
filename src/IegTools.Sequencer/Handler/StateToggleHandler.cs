namespace IegTools.Sequencer.Handler;

/// <summary>
/// Toggles the sequence between set-state and reset-state
/// if the condition for set or reset is met.
/// The set-condition is dominant, that means if the set-condition and reset-condition is met,
/// the sequence will be set to the set-state.
/// </summary>
public class StateToggleHandler : HandlerBase
{
    private readonly StateTransitionHandler _setToHandler;
    private readonly StateTransitionHandler _setFromHandler;

    /// <summary>
    /// Creates a new instance of the <see cref="StateToggleHandler"/>
    /// </summary>
    /// <param name="fromState">The sequence-state to reset to</param>
    /// <param name="toState">The sequence-state to set to</param>
    /// <param name="dominantSetToCondition">The dominant set-condition that must be fulfilled to execute the state-transition from reset to set-state</param>
    /// <param name="setFromCondition">The reset-condition that must be fulfilled to execute the state-transition from set to reset-state</param>
    /// <param name="setToAction">The action that will be executed after the set-state-transition</param>
    /// <param name="setFromAction">The action that will be executed after the reset-state-transition</param>
    /// <param name="description">The transition description (for debugging or just to describe what is it for)</param>
    public StateToggleHandler(
        string     fromState,           string     toState,
        Func<bool> dominantSetToCondition, Func<bool> setFromCondition,
        Action     setToAction = null,     Action     setFromAction = null,
        string description = "")
        : base(dominantSetToCondition, setToAction, description)
    {
        Name      = "Toggle States";
        FromState = fromState;
        ToState   = toState;

        _setToHandler   = new StateTransitionHandler(fromState, toState, dominantSetToCondition, setToAction, description);
        _setFromHandler = new StateTransitionHandler(toState, fromState, setFromCondition, setFromAction, description);
    }


    /// <summary>
    /// The Reset-State
    /// </summary>
    public string FromState { get; }

    /// <summary>
    /// The Set-State
    /// </summary>
    public string ToState   { get; }


    /// <summary>
    /// Returns a string representation of the handler-state
    /// </summary>
    public override string ToString() =>
        $"{Name}: {FromState} -> {ToState}";


    /// <inheritdoc />
    public override bool IsRegisteredState(string state) =>
        state.IsIn(FromState, ToState);

    /// <summary>
    /// Returns true if the sequence met the specified state and the set or reset condition is fulfilled
    /// </summary>
    /// <param name="sequence">The sequence</param>
    public override bool IsConditionFulfilled(ISequence sequence) =>
        _setToHandler.IsConditionFulfilled(sequence) || _setFromHandler.IsConditionFulfilled(sequence);


    /// <summary>
    /// Executes the specified action and transitions to the new state
    /// </summary>
    /// <param name="sequence">The sequence</param>
    public override void ExecuteAction(ISequence sequence)
    {
        using var scope = Logger?.GetSequenceLoggerScope(this, "Execute Action");

        // Hack to inject the sequence into the internal handler
        _setToHandler.Sequence = sequence;

        if (_setToHandler.IsConditionFulfilled(sequence))
        {
            Logger?.LogDebug(Logger.EventId, "{Handler} -> set state {SetState}", Name, ToState);

            _setToHandler.ExecuteAction(sequence);
            return;
        }

        // Hack to inject the sequence into the internal handler
        _setFromHandler.Sequence = sequence;

        var lockReset = Condition?.Invoke() ?? false;
        if (!lockReset && _setFromHandler.IsConditionFulfilled(sequence))
        {
            Logger?.LogDebug(Logger.EventId, "{Handler} -> reset to state {FromState}", Name, FromState);

            _setFromHandler.ExecuteAction(Sequence);
        }
    }
}
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
    private readonly StateTransitionHandler _resetSetHandler;
    private readonly StateTransitionHandler _setResetHandler;

    /// <summary>
    /// Creates a new instance of the <see cref="ToggleStatesHandler"/>
    /// </summary>
    /// <param name="resetState">The sequence-state to reset to</param>
    /// <param name="setState">The sequence-state to set to</param>
    /// <param name="dominantSetCondition">The dominant set-condition that must be fulfilled to execute the state-transition from reset to set-state</param>
    /// <param name="resetCondition">The reset-condition that must be fulfilled to execute the state-transition from set to reset-state</param>
    /// <param name="setAction">The action that will be executed after the set-state-transition</param>
    /// <param name="resetAction">The action that will be executed after the reset-state-transition</param>
    /// <param name="description">The transition description (for debugging or just to describe what is it for)</param>
    public ToggleStatesHandler(
        string     resetState,           string     setState,
        Func<bool> dominantSetCondition, Func<bool> resetCondition,
        Action     setAction = null,     Action     resetAction = null,
        string description = "")
        : base(dominantSetCondition, setAction, description)
    {
        Name             = "Toggle States";
        ResetToggleState = resetState;
        SetToggleState   = setState;

        _resetSetHandler = new StateTransitionHandler(resetState, setState, dominantSetCondition, setAction, description);
        _setResetHandler = new StateTransitionHandler(setState, resetState, resetCondition, resetAction, description);
    }


    /// <summary>
    /// The Reset-State
    /// </summary>
    public string ResetToggleState { get; }

    /// <summary>
    /// The Set-State
    /// </summary>
    public string SetToggleState   { get; }


    /// <summary>
    /// Returns a string representation of the handler-state
    /// </summary>
    public override string ToString() =>
        $"{Name}: {ResetToggleState} -> {SetToggleState}";


    /// <inheritdoc />
    public override bool IsRegisteredState(string state) =>
        state == ResetToggleState || state == SetToggleState;

    /// <summary>
    /// Returns true if the sequence met the specified state and the set or reset condition is fulfilled
    /// </summary>
    /// <param name="sequence">The sequence</param>
    public override bool IsConditionFulfilled(ISequence sequence) =>
        _resetSetHandler.IsConditionFulfilled(sequence) || _setResetHandler.IsConditionFulfilled(sequence);


    /// <summary>
    /// Executes the specified action and transitions to the new state
    /// </summary>
    /// <param name="sequence">The sequence</param>
    public override void ExecuteAction(ISequence sequence)
    {
        using var scope = Logger?.GetSequenceLoggerScope(this, "Execute Action");

        // Hack to inject the sequence into the internal handler
        _resetSetHandler.Sequence = sequence;

        if (_resetSetHandler.IsConditionFulfilled(sequence))
        {
            Logger?.LogDebug(Logger.EventId, "{Handler} -> set state {SetState}", Name, SetToggleState);

            _resetSetHandler.ExecuteAction(sequence);
            return;
        }

        // Hack to inject the sequence into the internal handler
        _setResetHandler.Sequence = sequence;

        var lockReset = Condition?.Invoke() ?? false;
        if (!lockReset && _setResetHandler.IsConditionFulfilled(sequence))
        {
            Logger?.LogDebug(Logger.EventId, "{Handler} -> reset to state {ResetToggleState}", Name, ResetToggleState);

            _setResetHandler.ExecuteAction(Sequence);
        }
    }
}
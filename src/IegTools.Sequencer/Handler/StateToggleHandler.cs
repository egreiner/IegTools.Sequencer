namespace IegTools.Sequencer.Handler;

/// <summary>
/// Toggles the sequence between set-state and reset-state
/// if the condition for set or reset is met.
/// The set-condition is dominant, that means if the set-condition and reset-condition is met,
/// the sequence will be set to the set-state.
/// </summary>
public class StateToggleHandler : HandlerBase, IHasToState
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
    public override IHandler SetSequence(ISequence sequence)
    {
        _setToHandler.SetSequence(sequence);
        _setFromHandler.SetSequence(sequence);
        return base.SetSequence(sequence);
    }

    /// <inheritdoc />
    public override bool IsRegisteredState(string state) =>
        state.IsIn(FromState, ToState);

    /// <summary>
    /// Returns true if the sequence met the specified state and the set or reset condition is fulfilled
    /// </summary>
    public override bool ExecuteActionAllowed() =>
        _setToHandler.ExecuteActionAllowed() || _setFromHandler.ExecuteActionAllowed();


    /// <summary>
    /// Executes the specified action and transitions to the new state
    /// </summary>
    public override void ExecuteAction()
    {
        using var scope = Logger?.GetSequenceLoggerScope(this, "Execute Action");

        if (_setToHandler.ExecuteActionAllowed())
        {
            Logger?.LogDebug(Logger.EventId, "{Handler} -> set state {SetState}", Name, ToState);

            _setToHandler.ExecuteAction();
            return;
        }

        // the dominant setToCondition locks the execution of the setFromAction
        var lockReset = Condition?.Invoke() ?? false;
        if (!lockReset && _setFromHandler.ExecuteActionAllowed())
        {
            Logger?.LogDebug(Logger.EventId, "{Handler} -> set to state {FromState}", Name, FromState);

            _setFromHandler.ExecuteAction();
        }
    }
}
namespace IegTools.Sequencer.Handler;

using System.Linq;
using Microsoft.Extensions.Logging;

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
        Name       = "Any-State Transition";
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

    
    /// <summary>
    /// Returns a string representation of the handler-state
    /// </summary>
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
        Logger?.Log(LogLevel.Debug, EventId, "{Method} - {Handler} -> from state '{StateFrom}' to state '{StateTo}'", "Execute Action", Name, Sequence.CurrentState, ToState);

        sequence.SetState(ToState);
        Action?.Invoke();
    }
}
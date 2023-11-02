﻿namespace IegTools.Sequencer.Handler;

/// <summary>
/// Invokes an specified action when the sequence is in the specified state
/// </summary>
public class StateActionHandler : HandlerBase
{
    public StateActionHandler(string state, Action action)
    {
        State  = state;
        Action = action;
    }

    
    /// <summary>
    /// The state the sequence must have to invoke the action
    /// </summary>
    public string State  { get; }


    public override string ToString() =>
        $"State-Action: {State}";


    /// <inheritdoc />
    public override bool IsRegisteredState(string state) =>
        state == State;

    /// <summary>
    /// Returns true if the sequence is in the specified state
    /// </summary>
    /// <param name="sequence">The sequence</param>
    public override bool IsConditionFulfilled(ISequence sequence) =>
        sequence.HasCurrentState(State) && IsConditionFulfilled();

    /// <summary>
    /// Executes the specified action
    /// </summary>
    /// <param name="sequence">The sequence</param>
    public override void ExecuteAction(ISequence sequence) =>
        Action?.Invoke();
}
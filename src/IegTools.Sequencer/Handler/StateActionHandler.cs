namespace IegTools.Sequencer.Handler;

using Microsoft.Extensions.Logging;

/// <summary>
/// Invokes an specified action when the sequence is in the specified state
/// </summary>
public class StateActionHandler : HandlerBase
{
    private bool _loggingDone;

    /// <summary>
    /// Initializes a new instance of the <see cref="StateActionHandler"/> class.
    /// This handler is responsible for linking a specific state to its corresponding action.
    /// </summary>
    /// <param name="state">This is the state to which the action is associated. It should be defined within the context of the sequence's possible states.</param>
    /// <param name="condition">The constraint that must be fulfilled that the sequence executes the action, default is true.</param>
    /// <param name="action">The action that will be executed when the sequence is in the defined state</param>
    /// <param name="description">The transition description (for debugging or just to describe what is it for)</param>
    public StateActionHandler(string state, Func<bool> condition, Action action, string description = "")
        : base(condition, action, description)
    {
        Name   = "State Action";
        State  = state;
    }

    
    /// <summary>
    /// The state the sequence must have to invoke the action
    /// </summary>
    public string State  { get; }


    /// <summary>
    /// Returns a string representation of the handler-state
    /// </summary>
    public override string ToString() =>
        $"State-Action: {State}";


    /// <inheritdoc />
    public override bool IsRegisteredState(string state) =>
        state == State;

    /// <summary>
    /// Returns true if the sequence is in the specified state
    /// </summary>
    /// <param name="sequence">The sequence</param>
    public override bool IsConditionFulfilled(ISequence sequence)
    {
        var result = sequence.HasCurrentState(State) && IsConditionFulfilled();

        if (!result) _loggingDone = false;

        return result;
    }

    /// <summary>
    /// Executes the specified action
    /// </summary>
    /// <param name="sequence">The sequence</param>
    public override void ExecuteAction(ISequence sequence)
    {
        if (!_loggingDone)
        {
            using var scope = GetLoggerScope("Execute Action");
            Logger?.Log(LogLevel.Debug, EventId, "{Handler} -> in state {StateTo}", Name, State);
            _loggingDone = true;
        }

        Action?.Invoke();
    }
}
namespace IegTools.Sequencer.Handler;

using Logging;

/// <summary>
/// The base handler
/// </summary>
public abstract class HandlerBase : IHandler
{
    private TimeSpan? _allowOnlyOnceTimeSpan;

    /// <summary>
    /// Creates a new instance of the <see cref="HandlerBase"/>
    /// </summary>
    /// <param name="condition">The condition that must be fulfilled to execute the state-transition</param>
    /// <param name="action">The action that will be executed after the transition</param>
    /// <param name="description">The transition description (for debugging or just to describe what is it for)</param>
    protected HandlerBase(Func<bool> condition, Action action, string description)
    {
        Condition   = condition;
        Action      = action;
        Description = description;
    }


    /// <inheritdoc />
    public ILoggerAdapter Logger { get; set; }

    /// <inheritdoc />
    public ISequence Sequence { get; set; }


    /// <summary>
    /// The Sequence-Configuration
    /// </summary>
    protected SequenceConfiguration Configuration => Sequence.Configuration;


    /// <inheritdoc />
    public string Name { get; protected init; }

    /// <inheritdoc />
    public string Description { get; }

    /// <inheritdoc />
    public Func<bool> Condition { get; set; }

    /// <inheritdoc />
    public Action Action { get; set; }


    /// <inheritdoc />
    public bool ResumeSequence { get; set; } = true;

    /// <inheritdoc />
    public DateTime LastExecutedAt { get; private set; }



    /// <summary>
    /// Returns true if a AllowOnlyOnceIn is set and the time is over
    /// </summary>
    private bool IsTimeOver =>
        _allowOnlyOnceTimeSpan == null || DateTime.Now > LastExecutedAt + _allowOnlyOnceTimeSpan;


    /// <inheritdoc />
    public abstract bool IsRegisteredState(string state);

    /// <inheritdoc />
    public abstract bool IsConditionFulfilled(ISequence sequence);

    /// <summary>
    /// Returns true if the handler condition is fulfilled
    /// </summary>
    protected bool IsConditionFulfilled() =>
        IsTimeOver && (Condition?.Invoke() ?? true);


    /// <inheritdoc />
    public abstract void ExecuteAction(ISequence sequence);


    /// <inheritdoc />
    public void AllowOnlyOnceIn(TimeSpan timeSpan) => _allowOnlyOnceTimeSpan = timeSpan;

    /// <inheritdoc />
    public bool ExecuteIfValid(ISequence sequence)
    {
        if (!this.IsConditionFulfilled(sequence)) return false;

        ExecuteAction(sequence);
        LastExecutedAt = DateTime.Now;

        return true;
    }
}
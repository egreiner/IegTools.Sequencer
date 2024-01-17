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
    public ISequence Sequence { get; private set; }


    /// <summary>
    /// The Sequence-Configuration
    /// </summary>
    protected SequenceConfiguration Configuration => Sequence.Configuration;


    /// <inheritdoc />
    public string Name { get; protected init; }

    /// <inheritdoc />
    public string Description { get; }

    /// <inheritdoc />
    public Func<bool> Condition { get; init; }

    /// <inheritdoc />
    public Action Action { get; init; }


    /// <inheritdoc />
    public bool ResumeSequence { get; set; } = true;

    /// <inheritdoc />
    public DateTime LastExecutedAt { get; private set; }


    /// <summary>
    /// The condition is fulfilled if the condition is null or the condition returns true
    /// </summary>
    protected bool ConditionSatisfied => Condition?.Invoke() ?? true;

    /// <summary>
    /// Returns true if a AllowOnlyOnceIn is set and the lock-time is over
    /// </summary>
    protected bool TimeLockExpired =>
        _allowOnlyOnceTimeSpan == null ||
        DateTime.Now > LastExecutedAt + _allowOnlyOnceTimeSpan;



    /// <inheritdoc />
    public abstract bool IsRegisteredState(string state);

    /// <inheritdoc />
    public abstract bool ExecuteActionAllowed();



    /// <inheritdoc />
    public abstract void ExecuteAction();


    /// <inheritdoc />
    public void AllowOnlyOnceIn(TimeSpan timeSpan) => _allowOnlyOnceTimeSpan = timeSpan;

    /// <inheritdoc />
    public bool ExecuteIfAllowed()
    {
        if (!ExecuteActionAllowed()) return false;

        ExecuteAction();

        return true;
    }

    /// <inheritdoc />
    public virtual IHandler SetSequence(ISequence sequence)
    {
        Sequence = sequence;
        return this;
    }

    /// <summary>
    /// Invokes the specified action,
    /// sets the last execution time
    /// and invokes the OnStateChangedAction if the state has changed
    /// </summary>
    protected void TryInvokeAction()
    {
        try
        {
            Action?.Invoke();

            if (Action is not null)
                LastExecutedAt = DateTime.Now;
        }
        catch (Exception e)
        {
            Logger?.LogError(Logger.EventId, e, "Try to invoke action failed");
        }
    }

    /// <summary>
    /// Sets the state of the sequence if the state is registered
    /// </summary>
    /// <param name="state">The state</param>
    protected void SetState(string state) =>
        Sequence?.SetState(state);
}
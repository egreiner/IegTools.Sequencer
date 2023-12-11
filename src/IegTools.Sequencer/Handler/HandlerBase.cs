namespace IegTools.Sequencer.Handler;

using System.Collections.Generic;
using Microsoft.Extensions.Logging;

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
    /// <param name="title">The transition title (for debugging or just to describe what is it for)</param>
    protected HandlerBase(Func<bool> condition, Action action, string title)
    {
        Condition = condition;
        Action    = action;
        Title     = title;
    }


    /// <inheritdoc />
    public string Name { get; protected init; }

    /// <inheritdoc />
    public string Title { get; }

    /// <inheritdoc />
    public SequenceConfiguration Configuration { get; set; }


    /// <inheritdoc />
    public Func<bool> Condition { get; set; }

    /// <inheritdoc />
    public Action Action        { get; set; }

    /// <inheritdoc />
    public DateTime LastExecutedAt { get; private set; }


    /// <inheritdoc />
    public bool ResumeSequence { get; set; } = true;


    /// <summary>
    /// The logger
    /// </summary>
    protected ILogger Logger => Configuration.Logger;

    /// <summary>
    /// The EventId for the logger
    /// </summary>
    protected EventId EventId => Configuration.EventId;

    /// <summary>
    /// The Sequence that this handler is bound to
    /// </summary>
    protected ISequence Sequence => Configuration.Sequence;



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

    /// <summary>
    /// Returns a logger scope
    /// </summary>
    /// <param name="methodName">The method-name</param>
    protected IDisposable GetLoggerScope(string methodName)
    {
        return Logger?.BeginScope(new Dictionary<string, object>
        {
            { "Title", Title },
            { "Method", methodName },
        });
    }
}
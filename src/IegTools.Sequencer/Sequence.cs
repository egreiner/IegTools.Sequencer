namespace IegTools.Sequencer;

using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Logging;

/// <summary>
/// A sequence
/// </summary>
public class Sequence : ISequence
{
    /// <inheritdoc />
    public ILoggerAdapter Logger { get; set; }

    /// <inheritdoc />
    public  SequenceConfiguration Configuration { get; private set; }

    /// <inheritdoc />
    public SequenceData Data { get; private set; }


    /// <inheritdoc />
    public string CurrentState { get; private set; }

    /// <inheritdoc />
    public string LastState { get; private set; }

    /// <inheritdoc />
    public Stopwatch Stopwatch { get; } = new();


    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    public override string ToString() => $"CurrentState: {CurrentState}";


    /// <inheritdoc />
    public bool HasCurrentState(string state) => CurrentState == state;

    /// <inheritdoc />
    public bool HasAnyCurrentState(params string[] states) => states.Contains(CurrentState);

    /// <inheritdoc />
    public bool IsRegisteredState(string state) =>
        Data.Handler.Any(x => x.IsRegisteredState(state));


    /// <inheritdoc />
    public virtual ISequence Run()
    {
        foreach (var handler in Data.Handler)
        {
            var executed = handler.ExecuteIfValid(this);
            if (executed && !handler.ResumeSequence)
                break;
        }

        return this;
    }

    /// <inheritdoc />
    public virtual async Task<ISequence> RunAsync() => await Task.Run(Run);


    /// <inheritdoc />
    public ISequence SetConfiguration(SequenceConfiguration configuration, SequenceData data)
    {
        Configuration = configuration;
        Data          = data;
        CurrentState  = configuration.InitialState;
        return this;
    }

    /// <inheritdoc />
    public ISequence SetState(string state, Func<bool> constraint)
    {
        if (constraint.Invoke())
            SetState(state);

        return this;
    }

    /// <inheritdoc />
    public ISequence SetState(string state)
    {
        LastState    = CurrentState;
        CurrentState = IsRegisteredState(state) ? state : CurrentState;

        TryToInvokeOnStateChangedAction();

        return this;
    }


    /// <summary>
    /// Try to invoke the OnStateChangedAction
    /// </summary>
    private void TryToInvokeOnStateChangedAction()
    {
        if (LastState == CurrentState) return;

        try
        {
            Data.OnStateChangedAction?.Invoke();
        }
        catch (Exception e)
        {
            Logger?.LogError(Logger.EventId, e, "Try to invoke OnStateChangedAction failed");
        }
    }
}
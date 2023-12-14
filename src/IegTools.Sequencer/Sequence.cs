namespace IegTools.Sequencer;

using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// A sequence
/// </summary>
public class Sequence : ISequence
{
    private SequenceData _data;

    /// <inheritdoc />
    public  SequenceConfiguration Configuration { get; private set; }


    /// <inheritdoc />
    public string CurrentState { get; private set; }

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
        _data.Handler.Any(x => x.IsRegisteredState(state));


    /// <inheritdoc />
    public virtual ISequence Run()
    {
        foreach (var handler in _data.Handler)
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
        _data         = data;
        CurrentState  = configuration.InitialState;
        return this;
    }

    /// <inheritdoc />
    public ISequence SetState(string state)
    {
        CurrentState = IsRegisteredState(state) ? state : Configuration.InitialState;
        return this;
    }

    /// <inheritdoc />
    public ISequence SetState(string state, Func<bool> constraint)
    {
        if (constraint.Invoke()) CurrentState = state;
        return this;
    }
}
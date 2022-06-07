namespace Ieg.Sequencer;

using System.Diagnostics;
using System.Linq;
using Descriptors;

public class Sequence : ISequence
{
    private SequenceConfiguration _configuration;
    
    
    /// <inheritdoc />
    public string CurrentState { get; private set; }

    /// <inheritdoc />
    public Stopwatch Stopwatch { get; } = new();

    
    /// <inheritdoc />
    public ISequence SetConfiguration(SequenceConfiguration configuration)
    {
        _configuration = configuration;
        CurrentState = configuration.InitialState;
        return this;
    }

    /// <inheritdoc />
    public ISequence SetState(string state)
    {
        CurrentState = state;
        return this;
    }
        
    /// <inheritdoc />
    public ISequence SetState(string state, Func<bool> constraint)
    {
        if (constraint.Invoke()) CurrentState = state;
        return this;
    }

    /// <inheritdoc />
    public virtual ISequence Run()
    {
        foreach (var descriptor in _configuration.Descriptors)
        {
            var executed = descriptor.ExecuteIfValid(this);
            if (!descriptor.ResumeSequence && executed) 
                break;
        }

        return this;
    }
}
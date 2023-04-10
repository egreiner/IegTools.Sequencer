﻿namespace IegTools.Sequencer;

using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

public class Sequence : ISequence
{
    private SequenceConfiguration _configuration;


    /// <inheritdoc />
    public string CurrentState { get; private set; }

    /// <inheritdoc />
    public Stopwatch Stopwatch { get; } = new();


    public override string ToString() => $"CurrentState: {CurrentState}";


    /// <inheritdoc />
    public bool HasCurrentState(string state) => CurrentState == state;

    /// <inheritdoc />
    public bool HasAnyCurrentState(params string[] states) => states.Contains(CurrentState);

    /// <inheritdoc />
    public bool IsRegisteredState(string state) =>
        _configuration.Handler.Any(x => x.IsRegisteredState(state));


    /// <inheritdoc />
    public virtual ISequence Run()
    {
        foreach (var handler in _configuration.Handler)
        {
            var executed = handler.ExecuteIfValid(this);
            if (!handler.ResumeSequence && executed) 
                break;
        }

        return this;
    }

    /// <inheritdoc />
    public virtual async Task<ISequence> RunAsync() => await Task.Run(Run);

        
    /// <inheritdoc />
    public ISequence SetConfiguration(SequenceConfiguration configuration)
    {
        _configuration = configuration;
        CurrentState   = configuration.InitialState;
        return this;
    }

    /// <inheritdoc />
    public ISequence SetState(string state)
    {
        CurrentState = IsRegisteredState(state) ? state : _configuration.InitialState;
        return this;
    }

    /// <inheritdoc />
    public ISequence SetState(string state, Func<bool> constraint)
    {
        if (constraint.Invoke()) CurrentState = state;
        return this;
    }
}
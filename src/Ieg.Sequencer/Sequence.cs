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
        if (ExecuteForceState()) return this;
            
        ExecuteStateTransitions();
        ExecuteStateActions();
        return this;
    }

    
    private bool ExecuteForceState()
    {
        var result = false;
        _configuration.Descriptors.OfType<ForceStateDescriptor>().ToList()
            .ForEach(state => result |= state.ExecuteIfValid(this));

        return result;
    }

    private void ExecuteStateTransitions() =>
        _configuration.Descriptors.OfType<StateTransitionDescriptor>().ToList()
            .ForEach(state => state.ExecuteIfValid(this));

    private void ExecuteStateActions() =>
        _configuration.Descriptors.OfType<StateActionDescriptor>().ToList()
            .ForEach(state => state.ExecuteIfValid(this));
}
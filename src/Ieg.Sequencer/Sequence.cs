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
        if (ExecuteForceStateDescriptor(GetForceStateDescriptor())) return this;
            
        ExecuteStateTransitionDescriptors();
        return this;
    }

      
    private ForceStateDescriptor GetForceStateDescriptor() =>
        _configuration.Descriptors.OfType<ForceStateDescriptor>()?.LastOrDefault();

    private bool ExecuteForceStateDescriptor(ForceStateDescriptor forceState)
    {
        var complied = forceState?.Constraint?.Invoke() ?? false;
        if (complied) CurrentState = forceState.State;

        return complied;
    }

    private void ExecuteStateTransitionDescriptors() =>
        _configuration.Descriptors.OfType<StateTransitionDescriptor>().ToList()
            .ForEach(ExecuteStateTransitionDescriptor);

    private void ExecuteStateTransitionDescriptor(StateTransitionDescriptor state)
    {
        if (state.ValidateTransition(CurrentState))
        {
            SetState(state.NextState);
            state.Action?.Invoke();
        }
    }
}
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
        if (ExecuteForceState(GetForceStateDescriptor())) return this;
            
        ExecuteStateTransitions();
        ExecuteStateActions();
        return this;
    }

    // TODO there is no reason why there shouldn't be more than one ForceDescriptors      
    private ForceStateDescriptor GetForceStateDescriptor() =>
        _configuration.Descriptors.OfType<ForceStateDescriptor>()?.LastOrDefault();

    private bool ExecuteForceState(ForceStateDescriptor forceState)
    {
        var complied = forceState?.Constraint?.Invoke() ?? false;
        if (complied) CurrentState = forceState.State;

        return complied;
    }

    private void ExecuteStateTransitions() =>
        _configuration.Descriptors.OfType<StateTransitionDescriptor>().ToList()
            .ForEach(ExecuteStateTransition);

    private void ExecuteStateTransition(StateTransitionDescriptor state)
    {
        if (state.ValidateTransition(CurrentState))
        {
            SetState(state.NextState);
            state.Action?.Invoke();
        }
    }

    private void ExecuteStateActions() =>
        _configuration.Descriptors.OfType<StateActionDescriptor>().ToList()
            .ForEach(ExecuteStateAction);

    private void ExecuteStateAction(StateActionDescriptor state)
    {
        if (state.ValidateState(CurrentState))
            state.Action?.Invoke();
    }
}
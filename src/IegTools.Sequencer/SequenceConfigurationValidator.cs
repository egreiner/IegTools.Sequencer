namespace IegTools.Sequencer;

using System.Collections.Generic;
using System.Linq;
using Descriptors;
using FluentValidation;

public class SequenceConfigurationValidator: AbstractValidator<SequenceConfiguration>
{
    public SequenceConfigurationValidator()
    {
        RuleFor(config => config.Descriptors.Count).GreaterThan(1);
        RuleFor(config => config.InitialState).NotEmpty();

        RuleFor(config => config)
            .Must(CorrectForceStateDescriptor)
            .WithMessage("Each ForceStateDescriptor must have a StateTransitionDescriptor counterpart.");
        
        RuleFor(config => config)
            .Must(CorrectFromState)
            .WithMessage(
                "Each 'FromState' must have an 'ToState' counterpart or an ForceState where it comes from.");
        
        RuleFor(config => config)
            .Must(CorrectToState)
            .WithMessage("Each 'ToState' must have an 'FromState' counterpart.");
    }

    /// <summary>
    /// Each 'Force.State' must have an corresponding 'Transition.FromState '
    /// otherwise you have created an dead end.
    /// </summary>
    private static bool CorrectForceStateDescriptor(SequenceConfiguration config)
    {
        var forceStatuses = config.Descriptors.OfType<ForceStateDescriptor>()
            .Where(x => IsValidState(x.State, config)).ToList();
        if (forceStatuses.Count == 0) return true;
        
        var transitions = config.Descriptors.OfType<StateTransitionDescriptor>().ToList();
        var missingDescriptors = new List<ForceStateDescriptor>();
        
        // for easy reading do not simplify this
        // each ForceStateDescriptor should have an counterpart StateTransitionDescriptor so that no dead end is reached
        foreach (var forceState in forceStatuses)
        {
            if (!transitions.Any(x => x.FromState == forceState.State))
                missingDescriptors.Add(forceState);
        }

        return missingDescriptors.Count == 0;
    }

    /// <summary>
    /// Each 'ToState' must have an corresponding 'FromState' counterpart,
    /// otherwise you have created an dead end.
    /// Use '!' as first character to tag an state as dead-end with purpose.
    /// </summary>
    private static bool CorrectToState(SequenceConfiguration config)
    {
        var transitions = config.Descriptors.OfType<StateTransitionDescriptor>().ToList();
        if (transitions.Count == 0) return true;
       
        var missingDescriptors = new List<StateTransitionDescriptor>();
        
        // for easy reading do not simplify this
        // each StateTransition should have an counterpart so that no dead end is reached
        foreach (var transition in transitions.Where(x => IsValidState(x.ToState, config)))
        {
            if (!transitions.Any(x => x.FromState == transition.ToState))
                missingDescriptors.Add(transition);
        }

        return missingDescriptors.Count == 0;
    }


    /// <summary>
    /// Each 'FromState' must have an corresponding 'ToState'  counterpart or a ForceState where it comes from,
    /// otherwise you have created an dead end.
    /// Use '!' as first character to tag an state as dead-end with purpose.
    /// </summary>
    private static bool CorrectFromState(SequenceConfiguration config)
    {
        var transitions = config.Descriptors.OfType<StateTransitionDescriptor>().ToList();
        var forceStatuses = config.Descriptors.OfType<ForceStateDescriptor>();
        if (transitions.Count == 0) return true;
       
        var missingDescriptors = new List<StateTransitionDescriptor>();
        
        // for easy reading do not simplify this
        // each StateTransition should have an counterpart so that no dead end is reached
        foreach (var transition in transitions.Where(x => IsValidState(x.FromState, config)))
        {
            if (!transitions.Any(x => x.ToState == transition.FromState) && !forceStatuses.Any(x => x.State == transition.FromState))
                missingDescriptors.Add(transition);
        }

        return missingDescriptors.Count == 0;
    }

    private static bool IsValidState(string state, SequenceConfiguration config)
    {
        return !state.StartsWith(config.IgnoreTag.ToString()) && !invalidStatuses().Contains(state);

        List<string> invalidStatuses() =>
            config.DisableValidationForStatuses?.ToList() ?? new List<string>();
    }
}
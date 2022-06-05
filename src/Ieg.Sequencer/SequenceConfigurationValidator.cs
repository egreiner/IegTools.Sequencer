namespace Ieg.Sequencer;

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

        RuleFor(config => config).Must(CorrectForceStateDescriptor).WithMessage("Each ForceStateDescriptor must have a StateTransitionDescriptor counterpart");
        RuleFor(config => config).Must(CorrectStateTransitionDescriptor).WithMessage("Each 'NextState' must have an 'CurrentState' counterpart");
    }

    /// <summary>
    /// Each 'NextState' must have an 'CurrentState'
    /// otherwise you have created an dead end.
    /// </summary>
    private static bool CorrectForceStateDescriptor(SequenceConfiguration config)
    {
        var forceStates = config.Descriptors.OfType<ForceStateDescriptor>().ToList();
        if (forceStates.Count == 0) return true;
        
        var transitions = config.Descriptors.OfType<StateTransitionDescriptor>().ToList();
        var missingDescriptors = new List<ForceStateDescriptor>();
        
        // for easy reading do not simplify this
        // each ForceStateDescriptor should have an counterpart StateTransitionDescriptor so that no dead end is reached
        foreach (var forceState in forceStates)
        {
            if (!transitions.Any(x => x.CurrentState == forceState.State))
                missingDescriptors.Add(forceState);
        }

        return missingDescriptors.Count == 0;
    }
    
    
    /// <summary>
    /// Each 'NextState' must have an 'CurrentState' (or a force-state)
    /// otherwise you have created an dead end.
    /// </summary>
    private static bool CorrectStateTransitionDescriptor(SequenceConfiguration config)
    {
        var transitions = config.Descriptors.OfType<StateTransitionDescriptor>().ToList();
        if (transitions.Count == 0) return true;
       
        var doNotValidate = config.DisableValidationForStates?.ToList() ?? new List<string>();
        var missingDescriptors = new List<StateTransitionDescriptor>();

        // for easy reading do not simplify this
        // each StateTransition should have an counterpart so that no dead end is reached
        foreach (var transition in transitions.Where(x => !doNotValidate.Contains(x.NextState)))
        {
            if (!transitions.Any(x => x.CurrentState == transition.NextState))
                missingDescriptors.Add(transition);
        }

        return missingDescriptors.Count == 0;
    }
}

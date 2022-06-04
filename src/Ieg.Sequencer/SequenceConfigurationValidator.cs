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

        // TODO add rule for 
        RuleFor(config => config).Must(CorrectStateDescriptors).WithMessage("Each goto state must have a come from state (or a force-state)");
    }

    /// <summary>
    /// Each 'NextState' must have an 'CurrentState' (or a force-state)
    /// otherwise you have created an dead end.
    /// </summary>
    private static bool CorrectStateDescriptors(SequenceConfiguration config) =>
        CorrectForceStateDescriptor(config) || CorrectStateTransitionDescriptor(config);

    /// <summary>
    /// Each 'NextState' must have an 'CurrentState' (or a force-state)
    /// otherwise you have created an dead end.
    /// </summary>
    private static bool CorrectForceStateDescriptor(SequenceConfiguration config)
    {
        // if there is an ForceStateDescriptor there is no dead end
        return config.Descriptors.Any(x => x is ForceStateDescriptor);
    }
    
    
    /// <summary>
    /// Each 'NextState' must have an 'CurrentState' (or a force-state)
    /// otherwise you have created an dead end.
    /// </summary>
    private static bool CorrectStateTransitionDescriptor(SequenceConfiguration config)
    {
        var transitions = config.Descriptors.Cast<StateTransitionDescriptor>().ToList();
       
        var missingDescriptors = new List<StateTransitionDescriptor>();
        
        // for easy reading do not simplify this
        // each StateTransition should have an counterpart so that no dead end is reached
        foreach (var transition in transitions)
        {
            if (!transitions.Any(x => x.CurrentState == transition.NextState))
                missingDescriptors.Add(transition);
        }

        return missingDescriptors.Count == 0;
    }
}

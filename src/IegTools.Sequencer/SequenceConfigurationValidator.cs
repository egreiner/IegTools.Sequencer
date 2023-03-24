namespace IegTools.Sequencer;

using System.Collections.Generic;
using System.Linq;
using Descriptors;
using FluentValidation;
using FluentValidation.Results;

public class SequenceConfigurationValidator: AbstractValidator<SequenceConfiguration>
{
    private static List<StateTransitionDescriptor> missingToStateDescriptors;
    private static List<ForceStateDescriptor> missingForceStateDescriptors;
    private static List<StateTransitionDescriptor> missingFromStateDescriptors;

    public SequenceConfigurationValidator()
    {
        RuleFor(config => config.Descriptors.Count).GreaterThan(1);
        RuleFor(config => config.InitialState).NotEmpty();
    }

    
    protected override bool PreValidate(ValidationContext<SequenceConfiguration> context, ValidationResult result)
    {
        var result1 = true;
        var config = context.InstanceToValidate;
        if (!CorrectForceStateDescriptor(config)) 
        {
            result.Errors.Add(new ValidationFailure("", 
                $"Each ForceStateDescriptor must have a StateTransitionDescriptor counterpart. Transition(s): {string.Join("; ", missingForceStateDescriptors)}"));
            result1 = false;
        }
        if (!CorrectFromState(config)) 
        {
            result.Errors.Add(new ValidationFailure("", 
                $"Each 'FromState' must have an 'ToState' counterpart or an ForceState where it comes from. Transition(s): {string.Join("; ", missingFromStateDescriptors)}"));
            result1 = false;
        }
        if (!CorrectToState(config)) 
        {
            result.Errors.Add(new ValidationFailure("", 
                $"Each 'ToState' must have an 'FromState' counterpart. Transition(s): {string.Join("; ", missingToStateDescriptors)}"));
            result1 = false;
        }
        return result1;
    }

    /// <summary>
    /// Each 'Force.State' must have an corresponding 'Transition.FromState '
    /// otherwise you have created an dead end.
    /// </summary>
    private static bool CorrectForceStateDescriptor(SequenceConfiguration config)
    {
        var forceStatuses = config.Descriptors.OfType<ForceStateDescriptor>()
            .Where(x => ShouldBeValidated(x.State, config)).ToList();
        if (forceStatuses.Count == 0) return true;
        
        var transitions = config.Descriptors.OfType<StateTransitionDescriptor>().ToList();
        missingForceStateDescriptors = new List<ForceStateDescriptor>();
        
        // for easy reading do not simplify this
        // each ForceStateDescriptor should have an counterpart StateTransitionDescriptor so that no dead end is reached
        foreach (var forceState in forceStatuses)
        {
            if (transitions.All(x => x.FromState != forceState.State))
                missingForceStateDescriptors.Add(forceState);
        }

        // if there are any transitions for the force-state remove it from the error-list
        var anyTransitions = config.Descriptors.OfType<AnyStateTransitionDescriptor>().ToList();
        foreach (var forceState in forceStatuses)
        {
            if (anyTransitions.All(x => x.FromStates.Contains(forceState.State)))
                missingForceStateDescriptors.Remove(forceState);
        }

        // if there are any transitions for the force-state remove it from the error-list
        var containsTransitions = config.Descriptors.OfType<ContainsStateTransitionDescriptor>().ToList();
        foreach (var forceState in forceStatuses)
        {
            if (containsTransitions.All(x => forceState.State.Contains(x.FromStateContains)))
                missingForceStateDescriptors.Remove(forceState);
        }

        return missingForceStateDescriptors.Count == 0;
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
       
        missingToStateDescriptors = new List<StateTransitionDescriptor>();
        
        // for easy reading do not simplify this
        // each StateTransition should have an counterpart so that no dead end is reached
        foreach (var transition in transitions.Where(x => ShouldBeValidated(x.ToState, config)))
        {
            if (transitions.All(x => x.FromState != transition.ToState))
                missingToStateDescriptors.Add(transition);
        }

        return missingToStateDescriptors.Count == 0;
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
       
        missingFromStateDescriptors = new List<StateTransitionDescriptor>();
        
        // for easy reading do not simplify this
        // each StateTransition should have an counterpart so that no dead end is reached
        foreach (var transition in transitions.Where(x => ShouldBeValidated(x.FromState, config)))
        {
            if (transitions.All(x => x.ToState != transition.FromState) && forceStatuses.All(x => x.State != transition.FromState))
                missingFromStateDescriptors.Add(transition);
        }

        return missingFromStateDescriptors.Count == 0;
    }

    private static bool ShouldBeValidated(string state, SequenceConfiguration config)
    {
        return !state.StartsWith(config.IgnoreTag.ToString()) && !disabledStatuses().Contains(state);

        IEnumerable<string> disabledStatuses() =>
            config.DisableValidationForStatuses?.ToList() ?? Enumerable.Empty<string>();
    }
}
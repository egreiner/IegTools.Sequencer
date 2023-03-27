namespace IegTools.Sequencer;

using System.Collections.Generic;
using System.Linq;
using Descriptors;
using FluentValidation;
using FluentValidation.Results;

public class SequenceConfigurationValidatorNew: AbstractValidator<SequenceConfiguration>
{
    private readonly Random _random = new();

    public SequenceConfigurationValidatorNew()
    {
        RuleFor(config => config.Descriptors.Count).GreaterThan(1);
        RuleFor(config => config.InitialState).NotEmpty();
    }

    
    protected override bool PreValidate(ValidationContext<SequenceConfiguration> context, ValidationResult result)
    {
        var config = context.InstanceToValidate;

        return HasValideInitialState(config, result) &
               HasValideDescriptors(config, result);
    }

    private static bool HasValideInitialState(SequenceConfiguration config, ValidationResult result)
    {
        if (config.Descriptors.OfType<StateTransitionDescriptor>().Any(x => x.FromState == config.InitialState))
            return true;

        ////if (config.Descriptors.OfType<AnyStateTransitionDescriptor>().Any(x => x.FromStates.Contains(config.InitialState)))
        ////    return true;

        var msg = "There is no corresponding transition with a 'FromState' for the 'InitialState'. Please check your configuration.";
        result.Errors.Add(new ValidationFailure("InitialState", $"{msg} "));
        return false;
    }

    private bool HasValideDescriptors(SequenceConfiguration config, ValidationResult result)
    {
        var remainingJobs = CheckDescriptors(config);

        if (remainingJobs.Count <= 0) return true;

        foreach (var job in remainingJobs)
        {
            var jobString = config.Descriptors.First(x => x.Id == job).ToString();
            var msg = "The following state is not correctly configured in the sequence. " +
                      "Please ensure that each 'FromState' has a corresponding 'ToState', and vice versa.";
            result.Errors.Add(new ValidationFailure("StateTransitions", $"{msg} Problematic job: [ {jobString} ]. "));
        }

        return false;
    }


    private HashSet<Guid> CheckDescriptors(SequenceConfiguration configuration)
    {
        var targetDescriptors   = GetDescriptorIds(configuration);
        var maxLoops = targetDescriptors.Count * 100;

        var sequence = new Sequence().SetConfiguration(configuration);

        // disable all actions
        configuration.Descriptors.ForEach(x => x.Action = null);

        ////var index = 0;
        var loop  = 0;
        while (maxLoops > loop++ && targetDescriptors.Count > 0)
        {
            ////if (index > configuration.Descriptors.Count -1) index = 0;

            foreach (var descriptor in configuration.Descriptors)
            {
                var currentId    = descriptor.Id;
                var currentState = sequence.CurrentState;

                // the condition should only be true for one descriptor per loop
                ////descriptor.Condition = () => configuration.Descriptors.IndexOf(descriptor) == index;
                descriptor.Condition = () => _random.Next(0, 10) >= 4;

                var actionExecuted = descriptor.ExecuteIfValid(sequence);
            
                if (actionExecuted && !sequence.HasCurrentState(currentState))
                    targetDescriptors.Remove(currentId);

                //if (actionExecuted)
                //    break;
            }

            ////index++;
        }

        ////foreach (var state in states.Where(state => ShouldBeIgnored(state, configuration)))
        ////    states.Remove(state);

        return targetDescriptors;
    }


    private static HashSet<Guid> GetDescriptorIds(SequenceConfiguration configuration) =>
        new(configuration.Descriptors.Select(descriptor => descriptor.Id));

    ////private static HashSet<string> GetValidationTargetStates(SequenceConfiguration configuration)
    ////{
    ////    var states = new List<string>();
    ////    foreach (var descriptorStates in configuration.Descriptors.Select(descriptor => descriptor.ValidationTargetStates))
    ////        states.AddRange(descriptorStates);

    ////    states.Add(configuration.InitialState);

    ////    // make a copy from states to enable states.Remove(state)
    ////    var result = new HashSet<string>(states);
    ////    foreach (var state in states.Distinct())
    ////    {
    ////        if (ShouldBeIgnored(state, configuration))
    ////            result.Remove(state);
    ////    }

    ////    return result;
    ////}


    private static bool ShouldBeIgnored(string state, SequenceConfiguration configuration)
    {
        return (state?.StartsWith(configuration.IgnoreTag.ToString()) ?? true) || 
               (disabledStatuses()?.Contains(state) ?? true);

        IEnumerable<string> disabledStatuses() =>
            configuration.DisableValidationForStatuses?.ToList() ?? Enumerable.Empty<string>();
    }
}
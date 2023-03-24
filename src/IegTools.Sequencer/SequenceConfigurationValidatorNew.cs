namespace IegTools.Sequencer;

using System.Collections.Generic;
using System.Linq;
using Descriptors;
using FluentValidation;
using FluentValidation.Results;

public class SequenceConfigurationValidatorNew: AbstractValidator<SequenceConfiguration>
{
    public SequenceConfigurationValidatorNew()
    {
        RuleFor(config => config.Descriptors.Count).GreaterThan(1);
        RuleFor(config => config.InitialState).NotEmpty();
    }

    
    protected override bool PreValidate(ValidationContext<SequenceConfiguration> context, ValidationResult result)
    {
        var config = context.InstanceToValidate;

        var x = CheckInitialState(config, result);

        return x & ValidateStateTransitions(config, result);
    }

    private static bool CheckInitialState(SequenceConfiguration config, ValidationResult result)
    {
        if (config.Descriptors.OfType<IHasFromState>().Any(x => x.FromState == config.InitialState))
            return true;

        var msg = "There is no corresponding transition with a 'FromState' for the 'InitialState'. Please check your configuration.";
        result.Errors.Add(new ValidationFailure("InitialState", $"{msg} "));
        return false;
    }

    private static bool ValidateStateTransitions(SequenceConfiguration config, ValidationResult result)
    {
        var notHandledStates = CheckTransitions(config);

        if (notHandledStates.Count > 0)
        {
            var states = string.Join(", ", notHandledStates);
            var msg = "The following state(s) are not correctly configured in the sequence. " +
                      "Please ensure that each 'FromState' has a corresponding 'ToState', and vice versa.";
            result.Errors.Add(new ValidationFailure("StateTransitions", $"{msg} Problematic state(s): [ {states} ]. "));
        }

        return notHandledStates.Count == 0;
    }


    private static HashSet<string> CheckTransitions(SequenceConfiguration configuration)
    {
        var states   = new HashSet<string>(configuration.AvailableStates);
        states.Remove(configuration.InitialState);

        var sequence = new Sequence();
        sequence.SetConfiguration(configuration);

        foreach (var descriptor in configuration.Descriptors)
        {
            // TODO descriptor.SetState(sequence);
            descriptor.ExecuteAction(sequence);
            states.Remove(sequence.CurrentState);
        }

        foreach (var state in states.Where(state => ShouldBeIgnored(state, configuration)))
            states.Remove(state);

        return states;
    }


    private static bool ShouldBeIgnored(string state, SequenceConfiguration config)
    {
        return state.StartsWith(config.IgnoreTag.ToString()) || disabledStatuses().Contains(state);

        IEnumerable<string> disabledStatuses() =>
            config.DisableValidationForStatuses?.ToList() ?? Enumerable.Empty<string>();
    }
}
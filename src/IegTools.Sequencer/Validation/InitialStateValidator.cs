namespace IegTools.Sequencer.Validation;

using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using Rules;

public class InitialStateValidator : ISequenceRuleValidator
{
    /// <inheritdoc />
    public bool Validate(ValidationContext<SequenceConfiguration> context, ValidationResult result)
    {
        var config = context.InstanceToValidate;
        if (!ShouldBeValidated(config.InitialState, config)) return true;

        if (RuleIsValidated(config)) return true;

        result.Errors.Add(new ValidationFailure("InitialState", 
            "The Initial-State must have an StateTransition counterpart"));
        
        return false;
    }

    private bool RuleIsValidated(SequenceConfiguration config) =>
        config.Rules.OfType<StateTransitionRule>().Any(x => config.InitialState == x.FromState) ||
        config.Rules.OfType<ContainsStateTransitionRule>().Any(x => config.InitialState.Contains(x.FromStateContains)) ||
        config.Rules.OfType<AnyStateTransitionRule>().Any(x => x.FromStates.Contains(config.InitialState));

    
    private static bool ShouldBeValidated(string state, SequenceConfiguration config)
    {
        return (!state?.StartsWith(config.IgnoreTag.ToString()) ?? true) && !disabledStatuses().Contains(state);

        IEnumerable<string> disabledStatuses() =>
            config.DisableValidationForStatuses?.ToList() ?? Enumerable.Empty<string>();
    }

}
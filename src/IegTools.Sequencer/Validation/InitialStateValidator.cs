namespace IegTools.Sequencer.Validation;

using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using Rules;

public class InitialStateValidator : ISequenceRuleValidator
{
    /// <inheritdoc />
    public bool Validate(ValidationContext<SequenceConfiguration> context, ValidationResult result)
    {
        if (RuleIsValidated(context.InstanceToValidate)) return true;

        result.Errors.Add(new ValidationFailure("InitialState", 
            "The Initial-State must have an StateTransition counterpart"));
        
        return false;
    }

    private bool RuleIsValidated(SequenceConfiguration config) =>
        config.Rules.OfType<StateTransitionRule>().Any(x => config.InitialState == x.FromState) ||
        config.Rules.OfType<ContainsStateTransitionRule>().Any(x => config.InitialState.Contains(x.FromStateContains)) ||
        config.Rules.OfType<AnyStateTransitionRule>().Any(x => x.FromStates.Contains(config.InitialState));
}
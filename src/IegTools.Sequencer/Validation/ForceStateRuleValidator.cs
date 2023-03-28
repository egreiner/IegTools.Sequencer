namespace IegTools.Sequencer.Validation;

using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using Rules;

public sealed class ForceStateRuleValidator : RuleValidatorBase, ISequenceRuleValidator
{
    private List<ForceStateRule> _rules;

    /// <inheritdoc />
    public bool Validate(ValidationContext<SequenceConfiguration> context, ValidationResult result)
    {
        if (RuleIsValidated(context.InstanceToValidate)) return true;

        result.Errors.Add(new ValidationFailure("ForceState",
            "Each Force-State must have an StateTransition counterpart.\n\r" +
            $"Violating rule(s): {string.Join("; ", _rules)}"));
        
        return false;
    }

    /// <summary>
    /// Each 'Force.State' must have an corresponding 'Transition.FromState(s) '
    /// otherwise you have created an dead-end.
    /// </summary>
    private bool RuleIsValidated(SequenceConfiguration config)
    {
        var result = RuleIsValidatedTo<ForceStateRule>(config);
        _rules = result.list.ToList();

        return result.isValid;
    }
}
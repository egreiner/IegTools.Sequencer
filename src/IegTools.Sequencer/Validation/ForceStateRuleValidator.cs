namespace IegTools.Sequencer.Validation;

using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using Rules;

public class ForceStateRuleValidator : ISequenceRuleValidator
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
        var forceStates = config.Rules.OfType<ForceStateRule>()
            .Where(x => ShouldBeValidated(x.ToState, config)).ToList();

        if (forceStates.Count == 0) return true;

        _rules = new List<ForceStateRule>();

        // for easy reading do not simplify this
        // each ForceStateRule should have an counterpart StateTransitionRule so that no dead-end is reached
        foreach (var forceState in forceStates.Where(x => ShouldBeValidated(x.ToState, config)))
        {
            if (config.Rules.OfType<StateTransitionRule>().All(x => forceState.ToState != x.FromState))
                _rules.Add(forceState);
        }

        if (_rules.Count == 0) return true;
        
        // remove all ForceStates that have a ContainsStateTransition counterpart
        foreach (var forceState in forceStates)
        {
            if (config.Rules.OfType<ContainsStateTransitionRule>().Any(x => forceState.ToState.Contains(x.FromStateContains)))
                _rules.Remove(forceState);
        }

        // remove all ForceStates that have a AnyStateTransition counterpart
        foreach (var forceState in forceStates)
        {
            if (config.Rules.OfType<AnyStateTransitionRule>().Any(x => x.FromStates.Contains(forceState.ToState)))
                _rules.Remove(forceState);
        }

        return _rules.Count == 0;
    }

   

    private static bool ShouldBeValidated(string state, SequenceConfiguration config)
    {
        return !state.StartsWith(config.IgnoreTag.ToString()) && !disabledStatuses().Contains(state);

        IEnumerable<string> disabledStatuses() =>
            config.DisableValidationForStatuses?.ToList() ?? Enumerable.Empty<string>();
    }
}
namespace IegTools.Sequencer.Validation;

using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using Rules;

public class StateTransitionRuleValidator : ISequenceRuleValidator
{
    private List<StateTransitionRule> _rulesFrom;
    private List<StateTransitionRule> _rulesTo;


    /// <inheritdoc />
    public bool Validate(ValidationContext<SequenceConfiguration> context, ValidationResult result)
    {
        var isValid = true;

        if (!RuleIsValidatedFrom(context.InstanceToValidate))
        {
            result.Errors.Add(new ValidationFailure("StateTransition",
                "Each 'FromState' must have an 'ToState' counterpart where it comes from (other Transition, Initial-State...)\n" +
                $"Violating rule(s): {string.Join("; ", _rulesFrom)}"));

            isValid = false;
        }

        if (!RuleIsValidatedTo(context.InstanceToValidate))
        {
            result.Errors.Add(new ValidationFailure("StateTransition",
                "Each 'ToState' must have an 'FromState' counterpart where it goes to (other Transition...)\n" +
                $"Violating Rule(s): {string.Join("; ", _rulesTo)}"));

            isValid = false;
        }

        return isValid;
    }


    /// <summary>
    /// Each 'FromState' must have an corresponding 'ToState' counterpart,
    /// or a ForceState where it comes from,
    /// or it must be the initial-state 
    /// otherwise you have created an dead-end.
    /// Use '!' as first character to tag an state as dead-end with purpose.
    /// </summary>
    private bool RuleIsValidatedFrom(SequenceConfiguration config)
    {
        var transitions = config.Rules.OfType<StateTransitionRule>().ToList();
        var allTransitions = config.Rules.OfType<IHasToState>().ToList();
        if (transitions.Count == 0) return true;

        _rulesFrom = new List<StateTransitionRule>();

        // for easy reading do not simplify this
        // each StateTransition should have an counterpart so that no dead-end is reached
        foreach (var transition in transitions.Where(x => ShouldBeValidated(x.FromState, config)))
        {
            if (allTransitions.All(x => transition.FromState != x.ToState) && 
                transition.FromState != config.InitialState)
                _rulesFrom.Add(transition);
            
            ////if (transitions.All(x => transition.FromState != x.ToState) &&
            ////    allTransitions.All(x => transition.FromState != x.ToState) &&
            ////    transition.FromState != config.InitialState)
            ////    _rulesFrom.Add(transition);
        }

        return _rulesFrom.Count == 0;
    }

        
    /// <summary>
    /// Each 'ToState' must have an corresponding 'FromState' counterpart,
    /// otherwise you have created an dead-end.
    /// Use '!' as first character to tag an state as dead-end with purpose.
    /// </summary>
    private bool RuleIsValidatedTo(SequenceConfiguration config)
    {
        var transitions = config.Rules.OfType<StateTransitionRule>().ToList();
        if (transitions.Count == 0) return true;

        _rulesTo = new List<StateTransitionRule>();

        // for easy reading do not simplify this
        // each StateTransition should have an counterpart so that no dead-end is reached
        foreach (var transition in transitions.Where(x => ShouldBeValidated(x.ToState, config)))
        {
            if (transitions.All(x => transition.ToState != x.FromState))
                _rulesTo.Add(transition);
        }

        // remove all ForceStates that have a ContainsStateTransition counterpart
        foreach (var transition in transitions.Where(x => ShouldBeValidated(x.ToState, config)))
        {
            if (config.Rules.OfType<ContainsStateTransitionRule>().Any(x => transition.ToState.Contains(x.FromStateContains)))
                _rulesTo.Remove(transition);
        }

        // remove all ForceStates that have a AnyStateTransition counterpart
        foreach (var transition in transitions.Where(x => ShouldBeValidated(x.ToState, config)))
        {
            if (config.Rules.OfType<AnyStateTransitionRule>().Any(x => x.FromStates.Contains(transition.ToState)))
                _rulesTo.Remove(transition);
        }

        return _rulesTo.Count == 0;
    }



    private static bool ShouldBeValidated(string state, SequenceConfiguration config)
    {
        return !state.StartsWith(config.IgnoreTag.ToString()) && !disabledStatuses().Contains(state);

        IEnumerable<string> disabledStatuses() =>
            config.DisableValidationForStatuses?.ToList() ?? Enumerable.Empty<string>();
    }
}
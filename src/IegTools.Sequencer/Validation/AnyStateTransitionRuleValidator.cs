namespace IegTools.Sequencer.Validation;

using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using Handler;

public sealed class AnyStateTransitionRuleValidator : RuleValidatorBase, ISequenceRuleValidator
{
    private List<AnyStateTransitionHandler> _rulesFrom;
    private List<AnyStateTransitionHandler> _rulesTo;


    /// <inheritdoc />
    public bool Validate(ValidationContext<SequenceConfiguration> context, ValidationResult result)
    {
        var isValid = true;

        if (!RuleIsValidatedFrom(context.InstanceToValidate))
        {
            result.Errors.Add(new ValidationFailure("AnyStateTransition",
                "Each 'FromState' of an AnyTransition must have an 'ToState' counterpart where it comes from (other Transition, Initial-State...)\n" +
                $"Violating rule(s): {string.Join("; ", _rulesFrom)}"));

            isValid = false;
        }

        if (!RuleIsValidatedTo(context.InstanceToValidate))
        {
            result.Errors.Add(new ValidationFailure("AnyStateTransition",
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
        var transitions = config.Rules.OfType<AnyStateTransitionHandler>().ToList();
        var allTransitions = config.Rules.OfType<IHasToState>().ToList();
        if (transitions.Count == 0) return true;

        _rulesFrom = new List<AnyStateTransitionHandler>();

        // for easy reading do not simplify this
        // each StateTransition should have an counterpart so that no dead-end is reached
        foreach (var transition in transitions)
        {
            foreach (var state in transition.FromStates.Where(x => ShouldBeValidated(x, config)))
            {
                if (allTransitions.All(x => state != x.ToState) &&
                    state != config.InitialState)
                    _rulesFrom.Add(transition);
            }
        }

        return _rulesFrom.Count == 0;    }

        
    /// <summary>
    /// Each 'ToState' must have an corresponding 'FromState' counterpart,
    /// otherwise you have created an dead-end.
    /// Use '!' as first character to tag an state as dead-end with purpose.
    /// </summary>
    private bool RuleIsValidatedTo(SequenceConfiguration config)
    {
        var result = RuleIsValidatedTo<AnyStateTransitionHandler>(config);
        _rulesTo = result.list.ToList();

        return result.isValid;
    }
}
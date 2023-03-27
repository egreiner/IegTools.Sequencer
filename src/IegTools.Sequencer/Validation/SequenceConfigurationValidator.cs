namespace IegTools.Sequencer.Validation;

using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using Rules;

public class SequenceConfigurationValidator : AbstractValidator<SequenceConfiguration>
{
    private static List<StateTransitionRule> missingToStates;
    private static List<StateTransitionRule> missingFromStates;
    private static List<AnyStateTransitionRule> missingAnyStates;
    private static List<ContainsStateTransitionRule> missingContainsStates;
    private static List<ForceStateRule> missingForces;

    public SequenceConfigurationValidator()
    {
        RuleFor(config => config.Rules.Count).GreaterThan(1).WithMessage("The sequence must have more than one rule");
        RuleFor(config => config.InitialState).NotEmpty();
    }


    protected override bool PreValidate(ValidationContext<SequenceConfiguration> context, ValidationResult result)
    {
        var isValide   = true;
        var validators = context.InstanceToValidate.RuleValidators.ToList();

        validators.ForEach(x => isValide &= x.Validate(context, result));

        return isValide;

        ////if (!CorrectAnyState(config))
        ////{
        ////    result.Errors.Add(new ValidationFailure("AnyStateTransition",
        ////        "Each 'FromState' of an AnyTransition must have an 'ToState' counterpart." +
        ////        $"Violating Rule(s): {string.Join("; ", missingAnyStates)}"));
        ////    result1 = false;
        ////}
        ////if (!CorrectContainsState(config))
        ////{
        ////    result.Errors.Add(new ValidationFailure("ContainsStateTransition",
        ////        "Each 'State-part' of an ContainsTransition must have an 'ToState' counterpart." +
        ////        $"Violating Rule(s): {string.Join("; ", missingContainsStates)}"));
        ////    result1 = false;
        ////}
    }
    
    /// <summary>
    /// Each 'ToState' must have an corresponding 'FromState' counterpart,
    /// otherwise you have created an dead-end.
    /// Use '!' as first character to tag an state as dead-end with purpose.
    /// </summary>
    private static bool CorrectToState(SequenceConfiguration config)
    {
        var transitions = config.Rules.OfType<StateTransitionRule>().ToList();
        if (transitions.Count == 0) return true;

        missingToStates = new List<StateTransitionRule>();

        // for easy reading do not simplify this
        // each StateTransition should have an counterpart so that no dead-end is reached
        foreach (var transition in transitions.Where(x => ShouldBeValidated(x.ToState, config)))
        {
            if (transitions.All(x => transition.ToState != x.FromState))
                missingToStates.Add(transition);
        }

        return missingToStates.Count == 0;
    }


    /// <summary>
    /// Each 'FromState' must have an corresponding 'ToState' counterpart,
    /// or a ForceState where it comes from,
    /// or it must be the initial-state 
    /// otherwise you have created an dead-end.
    /// Use '!' as first character to tag an state as dead-end with purpose.
    /// </summary>
    private static bool CorrectFromState(SequenceConfiguration config)
    {
        var transitions = config.Rules.OfType<StateTransitionRule>().ToList();
        var allTransitions = config.Rules.OfType<IHasToState>();
        if (transitions.Count == 0) return true;

        missingFromStates = new List<StateTransitionRule>();

        // for easy reading do not simplify this
        // each StateTransition should have an counterpart so that no dead-end is reached
        foreach (var transition in transitions.Where(x => ShouldBeValidated(x.FromState, config)))
        {
            if (transitions.All(x => transition.FromState != x.ToState) &&
                allTransitions.All(x => transition.FromState != x.ToState) &&
                transition.FromState != config.InitialState)
                missingFromStates.Add(transition);
        }

        return missingFromStates.Count == 0;
    }

    /// <summary>
    /// Each 'FromState' must have an corresponding 'ToState' counterpart,
    /// or a ForceState where it comes from,
    /// or it must be the initial-state 
    /// otherwise you have created an dead-end.
    /// Use '!' as first character to tag an state as dead-end with purpose.
    /// </summary>
    private static bool CorrectAnyState(SequenceConfiguration config)
    {
        var transitions = config.Rules.OfType<AnyStateTransitionRule>().ToList();
        var allTransitions = config.Rules.OfType<IHasToState>();
        if (transitions.Count == 0) return true;

        missingAnyStates = new List<AnyStateTransitionRule>();

        // for easy reading do not simplify this
        // each StateTransition should have an counterpart so that no dead-end is reached
        foreach (var transition in transitions)
        {
            foreach (var state in transition.FromStates.Where(x => ShouldBeValidated(x, config)))
            {
                if (transitions.All(x => state != x.ToState) &&
                    allTransitions.All(x => state != x.ToState) &&
                    state != config.InitialState)
                    missingAnyStates.Add(transition);
            }
        }

        return missingFromStates.Count == 0;
    }

    /// <summary>
    /// Each 'FromState' must have an corresponding 'ToState' counterpart,
    /// or a ForceState where it comes from,
    /// or it must be the initial-state 
    /// otherwise you have created an dead-end.
    /// Use '!' as first character to tag an state as dead-end with purpose.
    /// </summary>
    private static bool CorrectContainsState(SequenceConfiguration config)
    {
        var transitions = config.Rules.OfType<ContainsStateTransitionRule>().ToList();
        ////var allTransitions = config.Rules.OfType<IHasToState>();
        if (transitions.Count == 0) return true;

        missingContainsStates = new List<ContainsStateTransitionRule>();

        // for easy reading do not simplify this
        // each StateTransition should have an counterpart so that no dead-end is reached
        foreach (var transition in transitions)
        {
            if (transitions.All(x => !x.ToState.Contains(transition.FromStateContains)) &&
                ////allTransitions.All(x => transition.FromState != x.ToState) &&
                !config.InitialState.Contains(transition.FromStateContains))
                missingContainsStates.Add(transition);
        }

        return missingFromStates.Count == 0;
    }


    private static bool ShouldBeValidated(string state, SequenceConfiguration config)
    {
        return !state.StartsWith(config.IgnoreTag.ToString()) && !disabledStatuses().Contains(state);

        IEnumerable<string> disabledStatuses() =>
            config.DisableValidationForStatuses?.ToList() ?? Enumerable.Empty<string>();
    }
}
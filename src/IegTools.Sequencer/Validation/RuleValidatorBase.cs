namespace IegTools.Sequencer.Validation;

using IegTools.Sequencer.Rules;
using System.Collections.Generic;
using System.Linq;

public class RuleValidatorBase
{
    protected static bool ShouldBeValidated(string state, SequenceConfiguration config)
    {
        return (!state?.StartsWith(config.IgnoreTag.ToString()) ?? true) && !disabledStatuses().Contains(state);

        IEnumerable<string> disabledStatuses() =>
            config.DisableValidationForStatuses?.ToList() ?? Enumerable.Empty<string>();
    }

            
    /// <summary>
    /// Each 'ToState' must have an corresponding 'FromState' counterpart,
    /// otherwise you have created an dead-end.
    /// Use '!' as first character to tag an state as dead-end with purpose.
    /// </summary>
    protected (bool isValid, IEnumerable<T> list) RuleIsValidatedTo<T>(SequenceConfiguration config) where T: IHasToState
    {
        var containsTransitions = config.Rules.OfType<T>().ToList();
        if (containsTransitions is null || containsTransitions.Count == 0) return (true, Enumerable.Empty<T>());

        var transitions = config.Rules.OfType<StateTransitionRule>().ToList();

        var rulesTo = new List<IHasToState>();

        // for easy reading do not simplify this
        // each StateTransition should have an counterpart so that no dead-end is reached
        foreach (var transition in containsTransitions.Where(x => ShouldBeValidated(x.ToState, config)))
        {
            if (transitions.All(x => transition.ToState != x.FromState))
                rulesTo.Add(transition);
        }

        if (rulesTo.Count == 0) return (true, Enumerable.Empty<T>());

        // remove all ForceStates that have a ContainsStateTransition counterpart
        foreach (var transition in containsTransitions.Where(x => ShouldBeValidated(x.ToState, config)))
        {
            if (config.Rules.OfType<ContainsStateTransitionRule>().Any(x => transition.ToState.Contains(x.FromStateContains)))
                rulesTo.Remove(transition);
        }

        // remove all ForceStates that have a AnyStateTransition counterpart
        foreach (var transition in containsTransitions.Where(x => ShouldBeValidated(x.ToState, config)))
        {
            if (config.Rules.OfType<AnyStateTransitionRule>().Any(x => x.FromStates.Contains(transition.ToState)))
                rulesTo.Remove(transition);
        }

        return (rulesTo.Count == 0, rulesTo.ConvertAll(x => (T)x));
    }
}
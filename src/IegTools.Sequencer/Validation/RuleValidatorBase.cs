﻿namespace IegTools.Sequencer.Validation;

using System.Collections.Generic;
using System.Linq;
using Handler;

public class RuleValidatorBase
{
    private List<IHasToState> rulesTo;


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
        if (containsTransitions is null || containsTransitions.Count == 0)
            return (true, Enumerable.Empty<T>());

        var transitions = config.Rules.OfType<StateTransitionHandler>().ToList();

        rulesTo = new List<IHasToState>();

        
        AddMissingTransitions(config, containsTransitions, transitions);
        
        if (rulesTo.Count == 0)
            return (true, Enumerable.Empty<T>());

        
        RemoveWithContainsTransitions(config, containsTransitions);
        
        if (rulesTo.Count == 0)
            return (true, Enumerable.Empty<T>());


        RemoveWithAnyTransitions(config, containsTransitions);

        return (rulesTo.Count == 0, rulesTo.ConvertAll(x => (T)x));
    }


    private void AddMissingTransitions<T>(SequenceConfiguration config, List<T> containsTransitions, List<StateTransitionHandler> transitions)
        where T : IHasToState
    {
        // for easy reading do not simplify this
        // each StateTransition should have an counterpart so that no dead-end is reached
        foreach (var transition in containsTransitions.Where(x => ShouldBeValidated(x.ToState, config)))
        {
            if (transitions.All(x => transition.ToState != x.FromState))
                rulesTo.Add(transition);
        }
    }

    private void RemoveWithAnyTransitions<T>(SequenceConfiguration config, List<T> containsTransitions)
        where T : IHasToState
    {
        foreach (var transition in containsTransitions.Where(x => ShouldBeValidated(x.ToState, config)))
        {
            if (config.Rules.OfType<AnyStateTransitionHandler>().Any(x => x.FromStates.Contains(transition.ToState)))
                rulesTo.Remove(transition);
        }
    }

    private void RemoveWithContainsTransitions<T>(SequenceConfiguration config, List<T> containsTransitions)
        where T : IHasToState
    {
        foreach (var transition in containsTransitions.Where(x => ShouldBeValidated(x.ToState, config)))
        {
            if (config.Rules.OfType<ContainsStateTransitionHandler>()
                .Any(x => transition.ToState.Contains(x.FromStateContains)))
                rulesTo.Remove(transition);
        }
    }
}
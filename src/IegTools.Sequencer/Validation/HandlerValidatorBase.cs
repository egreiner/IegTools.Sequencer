namespace IegTools.Sequencer.Validation;

using System.Collections.Generic;
using System.Linq;
using Handler;

public class HandlerValidatorBase
{
    private List<IHasToState> handlerTo;


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
    protected (bool isValid, IEnumerable<T> list) HandlerIsValidatedTo<T>(SequenceConfiguration config) where T: IHasToState
    {
        var containsTransitions = config.Handler.OfType<T>().ToList();
        if (containsTransitions is null || containsTransitions.Count == 0)
            return (true, Enumerable.Empty<T>());

        var transitions = config.Handler.OfType<StateTransitionHandler>().ToList();

        handlerTo = new List<IHasToState>();

        
        AddMissingTransitions(config, containsTransitions, transitions);
        
        if (handlerTo.Count == 0)
            return (true, Enumerable.Empty<T>());

        
        RemoveWithContainsTransitions(config, containsTransitions);
        
        if (handlerTo.Count == 0)
            return (true, Enumerable.Empty<T>());


        RemoveWithAnyTransitions(config, containsTransitions);

        return (handlerTo.Count == 0, handlerTo.ConvertAll(x => (T)x));
    }


    private void AddMissingTransitions<T>(SequenceConfiguration config, List<T> containsTransitions, List<StateTransitionHandler> transitions)
        where T : IHasToState
    {
        // for easy reading do not simplify this
        // each StateTransition should have an counterpart so that no dead-end is reached
        foreach (var transition in containsTransitions.Where(x => ShouldBeValidated(x.ToState, config)))
        {
            if (transitions.All(x => transition.ToState != x.FromState))
                handlerTo.Add(transition);
        }
    }

    private void RemoveWithAnyTransitions<T>(SequenceConfiguration config, List<T> containsTransitions)
        where T : IHasToState
    {
        foreach (var transition in containsTransitions.Where(x => ShouldBeValidated(x.ToState, config)))
        {
            if (config.Handler.OfType<AnyStateTransitionHandler>().Any(x => x.FromStates.Contains(transition.ToState)))
                handlerTo.Remove(transition);
        }
    }

    private void RemoveWithContainsTransitions<T>(SequenceConfiguration config, List<T> containsTransitions)
        where T : IHasToState
    {
        foreach (var transition in containsTransitions.Where(x => ShouldBeValidated(x.ToState, config)))
        {
            if (config.Handler.OfType<ContainsStateTransitionHandler>()
                .Any(x => transition.ToState.Contains(x.FromStateContains)))
                handlerTo.Remove(transition);
        }
    }
}
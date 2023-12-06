namespace IegTools.Sequencer.Validation;

using System.Collections.Generic;
using System.Linq;
using Handler;

/// <summary>
/// The handler validator base class.
/// </summary>
public class HandlerValidatorBase
{
    private List<IHasToState> _handlerTo;


    /// <summary>
    /// Returns true if the state should be validated.
    /// </summary>
    /// <param name="state">The specified state</param>
    /// <param name="config">The sequence-configuration</param>
    /// <returns></returns>
    protected static bool ShouldBeValidated(string state, SequenceConfiguration config)
    {
        return (!state?.StartsWith(config.IgnoreTag.ToString()) ?? true) && !disabledStates().Contains(state);

        IEnumerable<string> disabledStates() =>
            config.DisableValidationForStates?.ToList() ?? Enumerable.Empty<string>();
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

        this._handlerTo = new List<IHasToState>();

        
        AddMissingTransitions(config, containsTransitions, transitions);
        
        if (this._handlerTo.Count == 0)
            return (true, Enumerable.Empty<T>());

        
        RemoveWithContainsTransitions(config, containsTransitions);
        
        if (this._handlerTo.Count == 0)
            return (true, Enumerable.Empty<T>());


        RemoveWithAnyTransitions(config, containsTransitions);

        return (this._handlerTo.Count == 0, this._handlerTo.ConvertAll(x => (T)x));
    }


    private void AddMissingTransitions<T>(SequenceConfiguration config, List<T> containsTransitions, List<StateTransitionHandler> transitions)
        where T : IHasToState
    {
        // for easy reading do not simplify this
        // each StateTransition should have an counterpart so that no dead-end is reached
        foreach (var transition in containsTransitions.Where(x => ShouldBeValidated(x.ToState, config)))
        {
            if (transitions.All(x => transition.ToState != x.FromState))
                this._handlerTo.Add(transition);
        }
    }

    private void RemoveWithAnyTransitions<T>(SequenceConfiguration config, List<T> containsTransitions)
        where T : IHasToState
    {
        foreach (var transition in containsTransitions.Where(x => ShouldBeValidated(x.ToState, config)))
        {
            if (config.Handler.OfType<AnyStateTransitionHandler>().Any(x => x.FromStates.Contains(transition.ToState)))
                this._handlerTo.Remove(transition);
        }
    }

    private void RemoveWithContainsTransitions<T>(SequenceConfiguration config, List<T> containsTransitions)
        where T : IHasToState
    {
        foreach (var transition in containsTransitions.Where(x => ShouldBeValidated(x.ToState, config)))
        {
            if (config.Handler.OfType<ContainsStateTransitionHandler>()
                .Any(x => transition.ToState.Contains(x.FromStateContains)))
                this._handlerTo.Remove(transition);
        }
    }
}
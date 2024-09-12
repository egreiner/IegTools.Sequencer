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
    /// TODO make this a (extension)method for SequenceBuilder?
    /// </summary>
    /// <param name="state">The specified state</param>
    /// <param name="builder">The sequence builder</param>
    protected static bool StateShouldBeValidated(string state, ISequenceBuilder builder)
    {
        return !stateShouldBeIgnored() && !disabledStates().Contains(state);

        bool stateShouldBeIgnored() =>
            state?.StartsWith(builder.Configuration.IgnoreTag.ToString()) ?? false;

        IEnumerable<string> disabledStates() =>
            builder.Configuration.DisableValidationForStates?.ToList() ?? Enumerable.Empty<string>();
    }

            
    /// <summary>
    /// Each 'ToState' must have a corresponding 'FromState' counterpart,
    /// otherwise you have created a dead-end.
    /// Use '!' as first character to tag a state as dead-end with purpose.
    /// </summary>
    protected (bool isValid, IEnumerable<T> list) HandlerIsValidatedTo<T>(SequenceBuilder builder) where T: IHasToState
    {
        var containsTransitions = builder.Data.Handler.OfType<T>().ToList();
        if (containsTransitions is null || containsTransitions.Count == 0)
            return (true, Enumerable.Empty<T>());

        var transitions = builder.Data.Handler.OfType<StateTransitionHandler>().ToList();

        this._handlerTo = new List<IHasToState>();

        
        AddMissingTransitions(builder, containsTransitions, transitions);
        
        if (this._handlerTo.Count == 0)
            return (true, Enumerable.Empty<T>());

        
        RemoveWithContainsTransitions(builder, containsTransitions);
        
        if (this._handlerTo.Count == 0)
            return (true, Enumerable.Empty<T>());


        RemoveWithAnyTransitions(builder, containsTransitions);

        return (this._handlerTo.Count == 0, this._handlerTo.ConvertAll(x => (T)x));
    }


    private void AddMissingTransitions<T>(SequenceBuilder builder, List<T> containsTransitions,
        List<StateTransitionHandler>                      transitions)
        where T : IHasToState
    {
        // for easy reading do not simplify this
        // each StateTransition should have a counterpart so that no dead-end is reached
        foreach (var transition in containsTransitions.Where(x => StateShouldBeValidated(x.ToState, builder)))
        {
            if (transitions.All(x => transition.ToState != x.FromState))
                this._handlerTo.Add(transition);
        }
    }

    private void RemoveWithAnyTransitions<T>(SequenceBuilder builder, List<T> containsTransitions)
        where T : IHasToState
    {
        foreach (var transition in containsTransitions.Where(x => StateShouldBeValidated(x.ToState, builder)))
        {
            if (builder.Data.Handler.OfType<AnyStateTransitionHandler>().Any(x => x.FromStates.Contains(transition.ToState)))
                this._handlerTo.Remove(transition);
        }
    }

    private void RemoveWithContainsTransitions<T>(SequenceBuilder builder, List<T> containsTransitions)
        where T : IHasToState
    {
        foreach (var transition in containsTransitions.Where(x => StateShouldBeValidated(x.ToState, builder)))
        {
            if (builder.Data.Handler.OfType<ContainsStateTransitionHandler>()
                .Any(x => transition.ToState.Contains(x.FromStateContains)))
                this._handlerTo.Remove(transition);
        }
    }
}
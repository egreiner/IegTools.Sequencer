namespace IegTools.Sequencer.Validation;

using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;
using Handler;

/// <summary>
/// The handler validator base class.
/// </summary>
public class HandlerValidatorBase
{
    private List<IHasToState> _handlerTo;


    /// <summary>
    /// Returns true if the state is valid.
    /// </summary>
    protected bool IsValid { get; set; } = true;


    /// <summary>
    /// Adds an error to the result.
    /// </summary>
    /// <param name="result">The validation result</param>
    /// <param name="propertyName">The name of the erroneous property</param>
    /// <param name="message">The validation error message</param>
    protected void AddError(ValidationResult result, string propertyName, string message)
    {
        result.Errors.Add(new ValidationFailure(propertyName, message));
        IsValid = false;
    }

    /// <summary>
    /// Returns true if the state should be validated.
    /// </summary>
    /// <param name="state">The specified state</param>
    /// <param name="builder">The sequence builder</param>
    protected static bool ShouldBeValidated(string state, SequenceBuilder builder)
    {
        return (!state?.StartsWith(builder.Configuration.IgnoreTag.ToString()) ?? true) && !disabledStates().Contains(state);

        IEnumerable<string> disabledStates() =>
            builder.Configuration.DisableValidationForStates?.ToList() ?? Enumerable.Empty<string>();
    }

            
    /// <summary>
    /// Each 'ToState' must have an corresponding 'FromState' counterpart,
    /// otherwise you have created an dead-end.
    /// Use '!' as first character to tag an state as dead-end with purpose.
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
        // each StateTransition should have an counterpart so that no dead-end is reached
        foreach (var transition in containsTransitions.Where(x => ShouldBeValidated(x.ToState, builder)))
        {
            if (transitions.All(x => transition.ToState != x.FromState))
                this._handlerTo.Add(transition);
        }
    }

    private void RemoveWithAnyTransitions<T>(SequenceBuilder builder, List<T> containsTransitions)
        where T : IHasToState
    {
        foreach (var transition in containsTransitions.Where(x => ShouldBeValidated(x.ToState, builder)))
        {
            if (builder.Data.Handler.OfType<AnyStateTransitionHandler>().Any(x => x.FromStates.Contains(transition.ToState)))
                this._handlerTo.Remove(transition);
        }
    }

    private void RemoveWithContainsTransitions<T>(SequenceBuilder builder, List<T> containsTransitions)
        where T : IHasToState
    {
        foreach (var transition in containsTransitions.Where(x => ShouldBeValidated(x.ToState, builder)))
        {
            if (builder.Data.Handler.OfType<ContainsStateTransitionHandler>()
                .Any(x => transition.ToState.Contains(x.FromStateContains)))
                this._handlerTo.Remove(transition);
        }
    }
}
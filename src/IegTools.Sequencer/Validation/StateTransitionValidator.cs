namespace IegTools.Sequencer.Validation;

using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using Handler;

/// <summary>
/// The state transition validator.
/// </summary>
public sealed class StateTransitionValidator : HandlerValidatorBase, IHandlerValidator
{
    private List<StateTransitionHandler> _handlerFrom;
    private List<StateTransitionHandler> _handlerTo;


    /// <inheritdoc />
    public bool Validate(ValidationContext<SequenceBuilder> context, ValidationResult result)
    {
        var isValid = true;

        if (!HandlerValidatedFrom(context.InstanceToValidate))
        {
            result.Errors.Add(new ValidationFailure("StateTransition",
                "Each 'FromState' must have an 'ToState' counterpart where it comes from (other Transition, Initial-State...)\n" +
                $"Violating handler: {string.Join("; ", _handlerFrom)}"));

            isValid = false;
        }

        if (!HandlerValidatedTo(context.InstanceToValidate))
        {
            result.Errors.Add(new ValidationFailure("StateTransition",
                "Each 'ToState' must have an 'FromState' counterpart where it goes to (other Transition...)\n" +
                $"Violating handler: {string.Join("; ", _handlerTo)}"));

            isValid = false;
        }

        return result.IsValid;
    }


    /// <summary>
    /// Each 'FromState' must have an corresponding 'ToState' counterpart,
    /// or a ForceState where it comes from,
    /// or it must be the initial-state 
    /// otherwise you have created an dead-end.
    /// Use '!' as first character to tag an state as dead-end with purpose.
    /// </summary>
    private bool HandlerValidatedFrom(SequenceBuilder builder)
    {
        var transitions = builder.Data.Handler.OfType<StateTransitionHandler>().ToList();
        var allTransitions = builder.Data.Handler.OfType<IHasToState>().ToList();
        if (transitions.Count == 0) return true;

        _handlerFrom = new List<StateTransitionHandler>();

        // for easy reading do not simplify this
        // each StateTransition should have an counterpart so that no dead-end is reached
        foreach (var transition in transitions.Where(x => ShouldBeValidated(x.FromState, builder)))
        {
            if (allTransitions.All(x => transition.FromState != x.ToState) && 
                transition.FromState != builder.Configuration.InitialState)
                _handlerFrom.Add(transition);
            
            ////if (transitions.All(x => transition.FromState != x.ToState) &&
            ////    allTransitions.All(x => transition.FromState != x.ToState) &&
            ////    transition.FromState != builder.InitialState)
            ////    _handlerFrom.Add(transition);
        }

        return _handlerFrom.Count == 0;
    }

        
    /// <summary>
    /// Each 'ToState' must have an corresponding 'FromState' counterpart,
    /// otherwise you have created an dead-end.
    /// Use '!' as first character to tag an state as dead-end with purpose.
    /// </summary>
    private bool HandlerValidatedTo(SequenceBuilder builder)
    {
        var result = HandlerIsValidatedTo<StateTransitionHandler>(builder);
        _handlerTo = result.list.ToList();

        return result.isValid;
    }
}
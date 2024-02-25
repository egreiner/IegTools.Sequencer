namespace IegTools.Sequencer.Validation;

using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using Handler;

/// <summary>
/// The contains state transition validator.
/// </summary>
public sealed class ContainsStateTransitionValidator : HandlerValidatorBase, IHandlerValidator
{
    private List<ContainsStateTransitionHandler> _handlerFrom;
    private List<ContainsStateTransitionHandler> _handlerTo;


    /// <inheritdoc />
    public bool Validate(ValidationContext<SequenceBuilder> context, ValidationResult result)
    {
        var isValid = true;

        if (!HandlerValidatedFrom(context.InstanceToValidate))
        {
            result.Errors.Add(new ValidationFailure("ContainsStateTransition",
                "Each 'State-part' of an ContainsTransition must have an 'ToState' counterpart." +
                $"Violating handler: {string.Join("; ", _handlerFrom)}"));

            ////result.Errors.Add(new ValidationFailure("AnyStateTransition",
            ////    "Each 'FromState' of an AnyTransition must have an 'ToState' counterpart where it comes from (other Transition, Initial-State...)\n" +
            ////    $"Violating handler: {string.Join("; ", _handlerFrom)}"));

            isValid = false;
        }

        if (!HandlerValidatedTo(context.InstanceToValidate))
        {
            result.Errors.Add(new ValidationFailure("ContainsStateTransition",
                "Each 'ToState' must have an 'FromState' counterpart where it goes to (other Transition...)\n" +
                $"Violating handler: {string.Join("; ", _handlerTo)}"));

            isValid = false;
        }

        return result.Errors.Count == 0;
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
        var transitions = builder.Data.Handler.OfType<ContainsStateTransitionHandler>().ToList();
        var allTransitions = builder.Data.Handler.OfType<IHasToState>().ToList();
        if (transitions.Count == 0) return true;

        _handlerFrom = new List<ContainsStateTransitionHandler>();

        // for easy reading do not simplify this
        // each StateTransition should have an counterpart so that no dead-end is reached
        foreach (var transition in transitions)
        {
            if (transitions.All(x => !x.ToState.Contains(transition.FromStateContains)) &&
                ////allTransitions.All(x => transition.FromState != x.ToState) &&
                !builder.Configuration.InitialState.Contains(transition.FromStateContains))
                _handlerFrom.Add(transition);
        }

        return _handlerFrom.Count == 0;    }

        
    /// <summary>
    /// Each 'ToState' must have an corresponding 'FromState' counterpart,
    /// otherwise you have created an dead-end.
    /// Use '!' as first character to tag an state as dead-end with purpose.
    /// </summary>
    private bool HandlerValidatedTo(SequenceBuilder builder)
    {
        var result = HandlerIsValidatedTo<ContainsStateTransitionHandler>(builder);
        _handlerTo = result.list.ToList();

        return result.isValid;
    }
}
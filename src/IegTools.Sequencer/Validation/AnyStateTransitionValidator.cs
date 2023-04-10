namespace IegTools.Sequencer.Validation;

using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using Handler;

public sealed class AnyStateTransitionValidator : HandlerValidatorBase, IHandlerValidator
{
    private List<AnyStateTransitionHandler> _handlerFrom;
    private List<AnyStateTransitionHandler> _handlerTo;


    /// <inheritdoc />
    public bool Validate(ValidationContext<SequenceConfiguration> context, ValidationResult result)
    {
        var isValid = true;

        if (!HandlerIsValidatedFrom(context.InstanceToValidate))
        {
            result.Errors.Add(new ValidationFailure("AnyStateTransition",
                "Each 'FromState' of an AnyTransition must have an 'ToState' counterpart where it comes from (other Transition, Initial-State...)\n" +
                $"Violating handler: {string.Join("; ", _handlerFrom)}"));

            isValid = false;
        }

        if (!HandlerIsValidatedTo(context.InstanceToValidate))
        {
            result.Errors.Add(new ValidationFailure("AnyStateTransition",
                "Each 'ToState' must have an 'FromState' counterpart where it goes to (other Transition...)\n" +
                $"Violating handler: {string.Join("; ", _handlerTo)}"));

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
    private bool HandlerIsValidatedFrom(SequenceConfiguration config)
    {
        var transitions = config.Handler.OfType<AnyStateTransitionHandler>().ToList();
        var allTransitions = config.Handler.OfType<IHasToState>().ToList();
        if (transitions.Count == 0) return true;

        _handlerFrom = new List<AnyStateTransitionHandler>();

        // for easy reading do not simplify this
        // each StateTransition should have an counterpart so that no dead-end is reached
        foreach (var transition in transitions)
        {
            foreach (var state in transition.FromStates.Where(x => ShouldBeValidated(x, config)))
            {
                if (allTransitions.All(x => state != x.ToState) &&
                    state != config.InitialState)
                    _handlerFrom.Add(transition);
            }
        }

        return _handlerFrom.Count == 0;    }

        
    /// <summary>
    /// Each 'ToState' must have an corresponding 'FromState' counterpart,
    /// otherwise you have created an dead-end.
    /// Use '!' as first character to tag an state as dead-end with purpose.
    /// </summary>
    private bool HandlerIsValidatedTo(SequenceConfiguration config)
    {
        var result = HandlerIsValidatedTo<AnyStateTransitionHandler>(config);
        _handlerTo = result.list.ToList();

        return result.isValid;
    }
}
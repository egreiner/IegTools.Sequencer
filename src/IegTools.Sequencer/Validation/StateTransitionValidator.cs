namespace IegTools.Sequencer.Validation;

using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using Handler;

public sealed class StateTransitionValidator : HandlerValidatorBase, IHandlerValidator
{
    private List<StateTransitionHandler> _handlerFrom;
    private List<StateTransitionHandler> _handlerTo;


    /// <inheritdoc />
    public bool Validate(ValidationContext<SequenceConfiguration> context, ValidationResult result)
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

        return isValid;
    }


    /// <summary>
    /// Each 'FromState' must have an corresponding 'ToState' counterpart,
    /// or a ForceState where it comes from,
    /// or it must be the initial-state 
    /// otherwise you have created an dead-end.
    /// Use '!' as first character to tag an state as dead-end with purpose.
    /// </summary>
    private bool HandlerValidatedFrom(SequenceConfiguration config)
    {
        var transitions = config.Handler.OfType<StateTransitionHandler>().ToList();
        var allTransitions = config.Handler.OfType<IHasToState>().ToList();
        if (transitions.Count == 0) return true;

        _handlerFrom = new List<StateTransitionHandler>();

        // for easy reading do not simplify this
        // each StateTransition should have an counterpart so that no dead-end is reached
        foreach (var transition in transitions.Where(x => ShouldBeValidated(x.FromState, config)))
        {
            if (allTransitions.All(x => transition.FromState != x.ToState) && 
                transition.FromState != config.InitialState)
                _handlerFrom.Add(transition);
            
            ////if (transitions.All(x => transition.FromState != x.ToState) &&
            ////    allTransitions.All(x => transition.FromState != x.ToState) &&
            ////    transition.FromState != config.InitialState)
            ////    _handlerFrom.Add(transition);
        }

        return _handlerFrom.Count == 0;
    }

        
    /// <summary>
    /// Each 'ToState' must have an corresponding 'FromState' counterpart,
    /// otherwise you have created an dead-end.
    /// Use '!' as first character to tag an state as dead-end with purpose.
    /// </summary>
    private bool HandlerValidatedTo(SequenceConfiguration config)
    {
        var result = HandlerIsValidatedTo<StateTransitionHandler>(config);
        _handlerTo = result.list.ToList();

        return result.isValid;
    }
}
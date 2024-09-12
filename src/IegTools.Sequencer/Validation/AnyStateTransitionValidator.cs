namespace IegTools.Sequencer.Validation;

using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using Handler;

/// <summary>
/// The any state transition validator.
/// </summary>
public sealed class AnyStateTransitionValidator : HandlerValidatorBase, IHandlerValidator
{
    private List<AnyStateTransitionHandler> _handlerFrom;
    private List<AnyStateTransitionHandler> _handlerTo;


    /// <inheritdoc />
    public bool Validate(ValidationContext<SequenceBuilder> context, ValidationResult result)
    {
        if (!HandlerIsValidatedFrom(context.InstanceToValidate))
            result.AddError("AnyStateTransition",
                "Each 'FromState' of an AnyTransition must have an 'ToState' counterpart where it comes from (other Transition, Initial-State...)\n" +
                $"Violating handler: {string.Join("; ", _handlerFrom)}");

        if (!HandlerIsValidatedTo(context.InstanceToValidate))
            result.AddError("AnyStateTransition",
                "Each 'ToState' must have an 'FromState' counterpart where it goes to (other Transition...)\n" +
                $"Violating handler: {string.Join("; ", _handlerTo)}");

        return result.IsValid;
    }


    /// <summary>
    /// Each 'FromState' must have an corresponding 'ToState' counterpart,
    /// or a ForceState where it comes from,
    /// or it must be the initial-state 
    /// otherwise you have created an dead-end.
    /// Use '!' as first character to tag an state as dead-end with purpose.
    /// </summary>
    private bool HandlerIsValidatedFrom(SequenceBuilder builder)
    {
        var transitions    = builder.Data.Handler.OfType<AnyStateTransitionHandler>().ToList();
        var allTransitions = builder.Data.Handler.OfType<IHasToState>().ToList();
        if (transitions.Count == 0) return true;

        _handlerFrom = new List<AnyStateTransitionHandler>();

        // for easy reading do not simplify this
        // each StateTransition should have an counterpart so that no dead-end is reached
        foreach (var transition in transitions)
        {
            foreach (var state in transition.FromStates.Where(x => StateShouldBeValidated(x, builder)))
            {
                if (allTransitions.All(x => state != x.ToState) &&
                    state != builder.Configuration.InitialState)
                    _handlerFrom.Add(transition);
            }
        }

        return _handlerFrom.Count == 0;    }

        
    /// <summary>
    /// Each 'ToState' must have an corresponding 'FromState' counterpart,
    /// otherwise you have created an dead-end.
    /// Use '!' as first character to tag an state as dead-end with purpose.
    /// </summary>
    private bool HandlerIsValidatedTo(SequenceBuilder builder)
    {
        var result = HandlerIsValidatedTo<AnyStateTransitionHandler>(builder);
        _handlerTo = result.list.ToList();

        return result.isValid;
    }
}
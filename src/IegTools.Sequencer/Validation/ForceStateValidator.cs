namespace IegTools.Sequencer.Validation;

using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using Handler;

public sealed class ForceStateValidator : HandlerValidatorBase, IHandlerValidator
{
    private List<ForceStateHandler> _handler;

    /// <inheritdoc />
    public bool Validate(ValidationContext<SequenceConfiguration> context, ValidationResult result)
    {
        if (HandlerIsValidated(context.InstanceToValidate)) return true;

        result.Errors.Add(new ValidationFailure("ForceState",
            "Each Force-State must have an StateTransition counterpart.\n\r" +
            $"Violating handler: {string.Join("; ", _handler)}"));
        
        return false;
    }

    /// <summary>
    /// Each 'Force.State' must have an corresponding 'Transition.FromState(s) '
    /// otherwise you have created an dead-end.
    /// </summary>
    private bool HandlerIsValidated(SequenceConfiguration config)
    {
        var result = HandlerIsValidatedTo<ForceStateHandler>(config);
        _handler = result.list.ToList();

        return result.isValid;
    }
}
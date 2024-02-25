namespace IegTools.Sequencer.Validation;

using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using Handler;

/// <summary>
/// The force state validator.
/// </summary>
public sealed class ForceStateValidator : HandlerValidatorBase, IHandlerValidator
{
    private List<ForceStateHandler> _handler;

    /// <inheritdoc />
    public bool Validate(ValidationContext<SequenceBuilder> context, ValidationResult result)
    {
        if (HandlerIsValidated(context.InstanceToValidate)) return true;

        result.AddError("ForceState",
            "Each Force-State must have an StateTransition counterpart.\n\r" +
            $"Violating handler: {string.Join("; ", _handler)}");
        
        return result.IsValid;
    }

    /// <summary>
    /// Each 'Force.State' must have an corresponding 'Transition.FromState(s) '
    /// otherwise you have created an dead-end.
    /// </summary>
    private bool HandlerIsValidated(SequenceBuilder builder)
    {
        var result = HandlerIsValidatedTo<ForceStateHandler>(builder);
        _handler = result.list.ToList();

        return result.isValid;
    }
}
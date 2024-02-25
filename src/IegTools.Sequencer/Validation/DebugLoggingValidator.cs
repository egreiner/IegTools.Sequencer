namespace IegTools.Sequencer.Validation;

using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using Handler;

/// <summary>
/// Validates if the transition/tasks have meaningful Description if the debug logging is activated.
/// </summary>
public sealed class DebugLoggingValidator : HandlerValidatorBase, IHandlerValidator
{
    private List<IHandler> _handler;

    /// <inheritdoc />
    public bool Validate(ValidationContext<SequenceBuilder> context, ValidationResult result)
    {
        var builder = context.InstanceToValidate;
        if (builder.Configuration.DisableValidation ||
            !builder.DebugLoggingActivated ||
            HandlerIsValidated(builder)) return true;

        result.Errors.Add(new ValidationFailure("Debug Logging",
            "Each transition/task in the sequence must have a meaningful description otherwise debug logging doesn't really make sense.\n\r" +
            $"Violating handler: {string.Join("; ", _handler)}"));

        return result.IsValid;
    }

    /// <summary>
    /// Each 'Force.State' must have an corresponding 'Transition.FromState(s) '
    /// otherwise you have created an dead-end.
    /// </summary>
    private bool HandlerIsValidated(ISequenceBuilder builder)
    {
        var notValid = builder.Data.Handler.Any(x =>
            string.IsNullOrEmpty(x.Description) ||
            x.Description.Contains(builder.DefaultDescription));

        _handler = builder.Data.Handler.Where(x =>
            string.IsNullOrEmpty(x.Description) ||
            x.Description.Contains(builder.DefaultDescription)).ToList();

        return !notValid;
    }
}
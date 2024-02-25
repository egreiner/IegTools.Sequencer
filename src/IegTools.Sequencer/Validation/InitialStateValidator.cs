namespace IegTools.Sequencer.Validation;

using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using Handler;

/// <summary>
/// The initial state validator.
/// </summary>
public sealed class InitialStateValidator : HandlerValidatorBase, IHandlerValidator
{
    /// <inheritdoc />
    public bool Validate(ValidationContext<SequenceBuilder> context, ValidationResult result)
    {
        var builder = context.InstanceToValidate;
        if (!ShouldBeValidated(builder.Configuration.InitialState, builder)) return true;

        if (string.IsNullOrEmpty(builder.Configuration.InitialState))
            result.AddError("InitialState", "The Initial-State must be defined");

        if (!HandlerIsValidated(builder))
            result.AddError("InitialState", "The Initial-State must have an StateTransition counterpart");

        return result.Errors.Count == 0;
    }

    private bool HandlerIsValidated(SequenceBuilder builder) =>
        builder.Data.Handler.OfType<StateTransitionHandler>().Any(x => builder.Configuration.InitialState == x.FromState) ||
        builder.Data.Handler.OfType<ContainsStateTransitionHandler>().Any(x => builder.Configuration.InitialState.Contains(x.FromStateContains)) ||
        builder.Data.Handler.OfType<AnyStateTransitionHandler>().Any(x => x.FromStates.Contains(builder.Configuration.InitialState));
}
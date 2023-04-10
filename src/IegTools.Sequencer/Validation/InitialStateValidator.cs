namespace IegTools.Sequencer.Validation;

using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using Handler;

public sealed class InitialStateValidator : HandlerValidatorBase, IHandlerValidator
{
    /// <inheritdoc />
    public bool Validate(ValidationContext<SequenceConfiguration> context, ValidationResult result)
    {
        var config = context.InstanceToValidate;
        if (!ShouldBeValidated(config.InitialState, config)) return true;

        if (HandlerIsValidated(config)) return true;

        result.Errors.Add(new ValidationFailure("InitialState", 
            "The Initial-State must have an StateTransition counterpart"));
        
        return false;
    }

    private bool HandlerIsValidated(SequenceConfiguration config) =>
        config.Handler.OfType<StateTransitionHandler>().Any(x => config.InitialState == x.FromState) ||
        config.Handler.OfType<ContainsStateTransitionHandler>().Any(x => config.InitialState.Contains(x.FromStateContains)) ||
        config.Handler.OfType<AnyStateTransitionHandler>().Any(x => x.FromStates.Contains(config.InitialState));
}
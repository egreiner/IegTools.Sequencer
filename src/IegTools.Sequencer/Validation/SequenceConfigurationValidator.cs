namespace IegTools.Sequencer.Validation;

using System.Linq;
using FluentValidation;
using FluentValidation.Results;

/// <summary>
/// The sequence configuration validator.
/// </summary>
public sealed class SequenceConfigurationValidator : AbstractValidator<SequenceConfiguration>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SequenceConfigurationValidator"/> class.
    /// </summary>
    public SequenceConfigurationValidator()
    {
        RuleFor(config => config.Handler.Count).GreaterThan(1).WithMessage("The sequence must have more than one handler");
        RuleFor(config => config.InitialState).NotEmpty();
    }


    /// <summary>
    /// Pre-validates the sequence configuration.
    /// </summary>
    /// <param name="context">The Validation context</param>
    /// <param name="result">The validation-result</param>
    /// <returns></returns>
    protected override bool PreValidate(ValidationContext<SequenceConfiguration> context, ValidationResult result)
    {
        var isValid = true;
        context.InstanceToValidate.Validators.ToList()
            .ForEach(x => isValid &= x.Validate(context, result));

        return isValid;
    }
}
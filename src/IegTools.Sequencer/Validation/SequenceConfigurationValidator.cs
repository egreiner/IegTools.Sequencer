namespace IegTools.Sequencer.Validation;

using System.Linq;
using FluentValidation;
using FluentValidation.Results;

public sealed class SequenceConfigurationValidator : AbstractValidator<SequenceConfiguration>
{
    public SequenceConfigurationValidator()
    {
        RuleFor(config => config.Handler.Count).GreaterThan(1).WithMessage("The sequence must have more than one handler");
        RuleFor(config => config.InitialState).NotEmpty();
    }


    protected override bool PreValidate(ValidationContext<SequenceConfiguration> context, ValidationResult result)
    {
        var isValide = true;
        context.InstanceToValidate.Validators.ToList()
            .ForEach(x => isValide &= x.Validate(context, result));

        return isValide;
    }
}
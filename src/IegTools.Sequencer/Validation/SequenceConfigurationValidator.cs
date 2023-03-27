﻿namespace IegTools.Sequencer.Validation;

using System.Linq;
using FluentValidation;
using FluentValidation.Results;

public class SequenceConfigurationValidator : AbstractValidator<SequenceConfiguration>
{
    public SequenceConfigurationValidator()
    {
        RuleFor(config => config.Rules.Count).GreaterThan(1).WithMessage("The sequence must have more than one rule");
        RuleFor(config => config.InitialState).NotEmpty();
    }


    protected override bool PreValidate(ValidationContext<SequenceConfiguration> context, ValidationResult result)
    {
        var isValide   = true;
        var validators = context.InstanceToValidate.RuleValidators.ToList();

        validators.ForEach(x => isValide &= x.Validate(context, result));

        return isValide;
    }
}
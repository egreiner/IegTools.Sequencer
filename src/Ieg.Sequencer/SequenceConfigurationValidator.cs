namespace Ieg.Sequencer;

using FluentValidation;

public class SequenceConfigurationValidator: AbstractValidator<SequenceConfiguration>
{
    public SequenceConfigurationValidator()
    {
        RuleFor(config => config.Descriptors.Count).GreaterThan(1);
        RuleFor(config => config.InitialState).NotEmpty();
    }
}

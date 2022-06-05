namespace Ieg.Sequencer;

using Descriptors;
using FluentValidation;

public class SequenceBuilder : ISequenceBuilder
{
    private readonly IValidator<SequenceConfiguration> _validator;
    private readonly SequenceConfiguration _configuration = new();

    private SequenceBuilder(IValidator<SequenceConfiguration> validator) =>
        _validator = validator;


    /// <summary>
    /// Creates a new Sequence-Builder for configuration in .NET 6 style.
    /// This is good for short crispy configs.
    /// </summary>
    public static ISequenceBuilder Create() =>
        new SequenceBuilder(new SequenceConfigurationValidator());

    /// <summary>
    /// Configures the sequence in .NET 5 style.
    /// This is good for larger complex configs.
    /// </summary>
    /// <param name="configurationActions">The action.</param>
    public static ISequenceBuilder Configure(Action<ISequenceBuilder> configurationActions)
    {
        var sequenceBuilder = Create();
        configurationActions.Invoke(sequenceBuilder);
        return sequenceBuilder;
    }


    /// <inheritdoc />
    public ISequence Build(string initialState)
    {
        _configuration.InitialState = initialState;

        if (!_configuration.DisableValidation)
            _validator?.ValidateAndThrow(_configuration);

        return new Sequence(_configuration);
    }

    
    /// <inheritdoc />
    public ISequenceBuilder AddDescriptor<T>(T descriptor) where T: SequenceDescriptor
    {
        _configuration.Descriptors.Add(descriptor);
        return this;
    }

    /// <inheritdoc />
    public ISequenceBuilder DisableValidation()
    {
        _configuration.DisableValidation = true;
        return this;
    }
}
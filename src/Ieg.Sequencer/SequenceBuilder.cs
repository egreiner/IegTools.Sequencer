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
        Create(new SequenceConfigurationValidator());

    /// <summary>
    /// Creates a new Sequence-Builder for configuration in .NET 6 style.
    /// This is good for short crispy configs.
    /// </summary>
    public static ISequenceBuilder Create(IValidator<SequenceConfiguration> validator) =>
        new SequenceBuilder(validator);

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

    /// <summary>
    /// Configures the sequence in .NET 5 style.
    /// This is good for larger complex configs.
    /// </summary>
    /// <param name="validator">Custom validator</param>
    /// <param name="configurationActions">The action.</param>
    public static ISequenceBuilder Configure(IValidator<SequenceConfiguration> validator, Action<ISequenceBuilder> configurationActions)
    {
        var sequenceBuilder = Create(validator);
        configurationActions.Invoke(sequenceBuilder);
        return sequenceBuilder;
    }


    /// <inheritdoc />
    public ISequence Build() =>
        Build<Sequence>();

    /// <inheritdoc />
    public ISequence Build<TSequence>() where TSequence : ISequence, new()
    {
        if (!_configuration.DisableValidation)
            _validator?.ValidateAndThrow(_configuration);

        return new TSequence().SetConfiguration(_configuration);
    }

    
    /// <inheritdoc />
    public ISequenceBuilder SetInitialState(string initialState)
    {
        _configuration.InitialState = initialState;
        return this;
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
namespace IegTools.Sequencer;

using Descriptors;
using FluentValidation;

public class SequenceBuilder : ISequenceBuilder
{
    private readonly IValidator<SequenceConfiguration> _validator;

    private SequenceBuilder(IValidator<SequenceConfiguration> validator) =>
        _validator = validator;

    
    /// <inheritdoc />
    public SequenceConfiguration Configuration { get; init; } = new();


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
        if (!Configuration.DisableValidation)
            _validator?.ValidateAndThrow(Configuration);

        return new TSequence().SetConfiguration(Configuration);
    }

    
    /// <inheritdoc />
    public ISequenceBuilder SetInitialState(string initialState)
    {
        Configuration.InitialState = initialState;
        return this;
    }
    
    /// <inheritdoc />
    public ISequenceBuilder AddDescriptor<T>(T descriptor) where T: IDescriptor
    {
        Configuration.Descriptors.Add(descriptor);
        return this;
    }

    /// <inheritdoc />
    public ISequenceBuilder DisableValidation()
    {
        Configuration.DisableValidation = true;
        return this;
    }

    /// <inheritdoc />
    public ISequenceBuilder DisableValidationForStatuses(params string[] statuses)
    {
        Configuration.DisableValidationForStatuses = statuses;
        return this;
    }
}
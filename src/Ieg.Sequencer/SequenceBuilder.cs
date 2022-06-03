namespace Ieg.Sequencer;

using Descriptors;
using FluentValidation;

public class SequenceBuilder : ISequenceBuilder
{
    private readonly SequenceConfiguration _configuration = new();

    private SequenceBuilder() {}


    /// <inheritdoc />
    public ISequenceBuilder AddDescriptor<T>(T descriptor) where T: SequenceDescriptor
    {
        _configuration.Descriptors.Add(descriptor);
        return this;
    }

    /// <inheritdoc />
    public ISequence Build(string initialState)
    {
        _configuration.InitialState = initialState;

        ////var validator = new SequenceConfigurationValidator();
        ////validator.ValidateAndThrow(_configuration);
        
        return new Sequence(_configuration);
    }


    /// <summary>
    /// Creates a new Sequence-Builder for configuration in .NET 6 style.
    /// This is good for short crispy configs.
    /// </summary>
    public static ISequenceBuilder Create() => new SequenceBuilder();

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
}
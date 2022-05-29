namespace Ieg.Sequencer;

using System;

public class SequenceBuilder : ISequenceBuilder
{
    private readonly SequenceConfiguration _configuration = new();
    private static string initialState;

    private SequenceBuilder() {}


    /// <inheritdoc />
    public ISequenceBuilder AddDescriptor<T>(T descriptor) where T: SequenceDescriptor
    {
        _configuration.Descriptors.Add(descriptor);
        return this;
    }

    /// <inheritdoc />
    public ISequence Build()
    {
        _configuration.InitialState = initialState;
        return new Sequence(_configuration);
    }


    /// <summary>
    /// Creates a new Sequence-Builder for configuration in .NET 6 style.
    /// This is good for short crispy configs.
    /// </summary>
    /// <param name="theInitialState">The initial-state of the sequence.</param>
    /// <returns>A Sequence-Builder.</returns>
    public static ISequenceBuilder Create(string theInitialState)
    {
        initialState = theInitialState;
        return new SequenceBuilder();
    }

    /// <summary>
    /// Configures the sequence in .NET 5 style.
    /// This is good for larger complex configs.
    /// </summary>
    /// <param name="theInitialState">The initial-state of the sequence.</param>
    /// <param name="configurationActions">The action.</param>
    /// <returns>The Sequence-Builder.</returns>
    public static ISequenceBuilder Configure(string theInitialState, Action<ISequenceBuilder> configurationActions)
    {
        var sequenceBuilder = Create(theInitialState);
        configurationActions.Invoke(sequenceBuilder);
        return sequenceBuilder;
    }

    /// <summary>
    /// Configures the sequence in .NET 5 style.
    /// This is good for larger complex configs.
    /// </summary>
    /// <param name="theInitialState">The initial-state (as enum) of the sequence.</param>
    /// <param name="configurationActions">The action.</param>
    /// <returns>The Sequence-Builder.</returns>
    public static ISequenceBuilder Configure(Enum theInitialState, Action<ISequenceBuilder> configurationActions) =>
        Configure(theInitialState.ToString(), configurationActions);
}
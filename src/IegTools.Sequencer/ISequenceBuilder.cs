namespace IegTools.Sequencer;

using Descriptors;

public interface ISequenceBuilder
{
    /// <summary>
    /// The sequence configuration
    /// </summary>
    SequenceConfiguration Configuration { get; init; }

    /// <summary>
    /// Builds a default sequence with the specified configuration
    /// </summary>
    ISequence Build();

    /// <summary>
    /// Builds a customized sequence with the specified configuration
    /// </summary>
    ISequence Build<TSequence>() where TSequence : ISequence, new();

    /// <summary>
    /// Sets the initial state
    /// </summary>
    /// <param name="initialState">The initial state</param>
    ISequenceBuilder SetInitialState(string initialState);

    /// <summary>
    /// Adds an sequence descriptor
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="descriptor">The descriptor.</param>
    ISequenceBuilder AddDescriptor<T>(T descriptor) where T: IDescriptor;

    /// <summary>
    /// Does not validate the sequence configuration on build
    /// </summary>
    ISequenceBuilder DisableValidation();

    /// <summary>
    /// Does not validate statuses that are in this list
    /// </summary>
    /// <param name="statuses">A list of statuses that should not be validated.</param>
    ISequenceBuilder DisableValidationForStatuses(params string[] statuses);
}
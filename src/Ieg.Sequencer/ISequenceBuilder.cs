namespace Ieg.Sequencer;

using Descriptors;

public interface ISequenceBuilder
{
    /// <summary>
    /// Builds a default sequence with the specified configuration.
    /// </summary>
    /// <param name="initialState">The state the sequence should start with.</param>
    ISequence Build(string initialState);

    /// <summary>
    /// Builds a customized sequence with the specified configuration.
    /// </summary>
    /// <param name="initialState">The state the sequence should start with.</param>
    ISequence Build<TSequence>(string initialState) where TSequence : ISequence, new();

    /// <summary>
    /// Adds an sequence descriptor.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="descriptor">The descriptor.</param>
    ISequenceBuilder AddDescriptor<T>(T descriptor) where T: SequenceDescriptor;

    /// <summary>
    /// Does not validate the sequence configuration on build.
    /// </summary>
    ISequenceBuilder DisableValidation();
}
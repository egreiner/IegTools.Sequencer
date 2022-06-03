namespace Ieg.Sequencer;

using Descriptors;

public interface ISequenceBuilder
{
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

    /// <summary>
    /// Builds a sequence with the specified configuration.
    /// </summary>
    /// <param name="initialState">The state the sequence should start with.</param>
    ISequence Build(string initialState);
}
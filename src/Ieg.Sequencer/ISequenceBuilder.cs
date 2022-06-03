namespace Ieg.Sequencer;

using Descriptors;

public interface ISequenceBuilder
{
    /// <summary>
    /// Builds a sequence with the specified configuration.
    /// </summary>
    /// <param name="initialState">The state the sequence should start with.</param>
    ISequence Build(string initialState);
        
    ISequenceBuilder AddDescriptor<T>(T descriptor) where T: SequenceDescriptor;
}
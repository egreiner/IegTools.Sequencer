namespace Ieg.Sequencer;

using Descriptors;

public interface ISequenceBuilder
{
    /// <summary>
    /// Builds a sequence with the specified configuration.
    /// </summary>
    ISequence Build();
        
    ISequenceBuilder AddDescriptor<T>(T descriptor) where T: SequenceDescriptor;
}
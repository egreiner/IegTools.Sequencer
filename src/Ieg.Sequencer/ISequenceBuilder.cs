namespace Ieg.Sequencer;

public interface ISequenceBuilder
{
    /// <summary>
    /// Builds a sequence with the specified configuration.
    /// TODO add configuration validation.
    /// </summary>
    ISequence Build();
        
    ISequenceBuilder AddDescriptor<T>(T descriptor) where T: SequenceDescriptor;
}
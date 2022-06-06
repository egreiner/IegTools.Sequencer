namespace Ieg.Sequencer.Descriptors;

public interface IDescriptor
{
    bool ValidateAction(ISequence sequence);
    void ExecuteAction(ISequence sequence);
    bool ExecuteIfValid(ISequence sequence);
}
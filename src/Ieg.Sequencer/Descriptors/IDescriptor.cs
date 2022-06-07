namespace Ieg.Sequencer.Descriptors;

public interface IDescriptor
{
    bool ResumeSequence { get; set; }

    bool ValidateAction(ISequence sequence);
    void ExecuteAction(ISequence sequence);
    bool ExecuteIfValid(ISequence sequence);
}
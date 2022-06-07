namespace Ieg.Sequencer.Descriptors;

public abstract class DescriptorBase : IDescriptor
{
    public bool ResumeSequence { get; set; } = true;
    
    public abstract bool ValidateAction(ISequence sequence);
    public abstract void ExecuteAction(ISequence sequence);

    
    public bool ExecuteIfValid(ISequence sequence)
    {
        var complied = ValidateAction(sequence);
        if (complied)  ExecuteAction(sequence);

        return complied;
    }
}
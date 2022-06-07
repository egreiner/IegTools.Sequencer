namespace Ieg.Sequencer.Descriptors;

/// <summary>
/// The base descriptor
/// </summary>
public abstract class DescriptorBase : IDescriptor
{
    /// <inheritdoc />
    public bool ResumeSequence { get; set; } = true;

    /// <inheritdoc />
    public abstract bool ValidateAction(ISequence sequence);

    /// <inheritdoc />
    public abstract void ExecuteAction(ISequence sequence);

    /// <inheritdoc />
    public bool ExecuteIfValid(ISequence sequence)
    {
        var complied = ValidateAction(sequence);
        if (complied)  ExecuteAction(sequence);

        return complied;
    }
}
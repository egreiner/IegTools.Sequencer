namespace IegTools.Sequencer.Descriptors;

/// <summary>
/// The base descriptor
/// </summary>
public abstract class DescriptorBase : IDescriptor
{
    /// <summary>
    /// The constraint that should be met to make the transition
    /// </summary>
    protected Func<bool> Constraint { get; init; }

    /// <inheritdoc />
    public bool ResumeSequence { get; set; } = true;


    /// <inheritdoc />
    public abstract bool IsRegisteredState(string state);

    /// <inheritdoc />
    public abstract bool ValidateAction(ISequence sequence);

    /// <inheritdoc />
    public abstract void ExecuteAction(ISequence sequence);

    /// <inheritdoc />
    public bool ExecuteIfValid(ISequence sequence)
    {
        var complied = ValidateAction(sequence);

        if (complied && !sequence.ValidationOnly) 
            ExecuteAction(sequence);

        return complied;
    }

    /// <summary>
    /// Returns true the sequence is in ValidationOnly-mode or if the descriptors constraint is fulfilled
    /// </summary>
    /// <param name="sequence"></param>
    protected bool IsConditionFulfilled(ISequence sequence) =>
        (sequence.ValidationOnly || (Constraint?.Invoke() ?? true));
}
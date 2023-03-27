namespace IegTools.Sequencer.Rules;

/// <summary>
/// The base descriptor
/// </summary>
public abstract class RuleBase : IDescriptor
{
    /// <inheritdoc />
    public Guid Id { get; } = Guid.NewGuid();

    /// <inheritdoc />
    public bool ResumeSequence { get; set; } = true;

    /////// <inheritdoc />
    ////public HashSet<string> ValidationTargetStates { get; set; } = new();

    /// <summary>
    /// The constraint that should be met to make the transition
    /// </summary>
    public Func<bool> Condition { get; set; }

    /// <summary>
    /// The action that will be invoked when the state transition will be executed
    /// </summary>
    public Action Action         { get; set; }


    /// <inheritdoc />
    public abstract bool IsRegisteredState(string state);

    /////// <inheritdoc />
    ////public abstract bool IsConditionFulfilled(ISequence sequence);

    /// <summary>
    /// Returns true the sequence is in ValidationOnly-mode or if the descriptors constraint is fulfilled
    /// </summary>
    /// <param name="sequence"></param>
    public virtual bool IsConditionFulfilled(ISequence sequence) =>
        Condition?.Invoke() ?? true;


    /// <inheritdoc />
    public abstract void ExecuteAction(ISequence sequence);

    /// <inheritdoc />
    public bool ExecuteIfValid(ISequence sequence)
    {
        var complied = IsConditionFulfilled(sequence);
        if (complied)  ExecuteAction(sequence);

        return complied;
    }
}
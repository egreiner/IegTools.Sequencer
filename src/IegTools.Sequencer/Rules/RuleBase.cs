namespace IegTools.Sequencer.Rules;

/// <summary>
/// The base rule
/// </summary>
public abstract class RuleBase : IRule
{
    /// <inheritdoc />
    public bool ResumeSequence { get; set; } = true;

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

    /// <summary>
    /// Returns true the sequence is in ValidationOnly-mode or if the rules constraint is fulfilled
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
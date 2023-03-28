namespace IegTools.Sequencer.Rules;

/// <summary>
/// Forces the sequence to the specified state if the condition is fulfilled
/// </summary>
public class ForceStateRule : RuleBase, IHasToState
{
    public ForceStateRule(string toState, Func<bool> condition, Action action)
    {
        ToState        = toState;
        Condition      = condition;
        ResumeSequence = false;
        Action         = action;
    }


    /// <summary>
    /// The state the sequence should be forced to
    /// </summary>
    public string ToState { get; }
    
    
    public override string ToString() =>
        $"Force-State: {ToState}";


    /// <inheritdoc />
    public override bool IsRegisteredState(string state) =>
        state == ToState;

    /// <summary>
    /// This rule is not dependent on the current sequence state.
    /// It depends on the condition only.
    /// </summary>
    /// <param name="sequence">The sequence</param>
    public override bool IsConditionFulfilled(ISequence sequence) =>
        !sequence.HasCurrentState(ToState) && (Condition?.Invoke() ?? false);

    /// <summary>
    /// Transitions to the new state
    /// </summary>
    /// <param name="sequence">The sequence</param>
    public override void ExecuteAction(ISequence sequence) =>
        sequence.SetState(ToState);
}
namespace IegTools.Sequencer.Descriptors;


/// <summary>
/// Invokes an specified action when the sequence is in the specified state
/// </summary>
public class StateActionDescriptor : DescriptorBase
{
    public StateActionDescriptor(string state, Action action)
    {
        State  = state;
        Action = action;

        ValidationTargetStates.Add(State);
    }

    
    /// <summary>
    /// The state the sequence must have to invoke the action
    /// </summary>
    public string State  { get; }

    /// <summary>
    /// The action to invoke
    /// </summary>
    public Action Action { get; }


    public override string ToString() => $"{State} (Action)";


    /// <inheritdoc />
    public override bool IsRegisteredState(string state) =>
        state == State;

    /// <summary>
    /// Returns true if the sequence is in the specified state
    /// </summary>
    /// <param name="sequence">The sequence</param>
    public override bool ValidateAction(ISequence sequence) =>
        sequence.HasCurrentState(State);

    /// <summary>
    /// Invokes the specified action
    /// </summary>
    /// <param name="sequence">The sequence</param>
    public override void ExecuteAction(ISequence sequence)
    {
        if (sequence.ValidationOnly) return;
        Action?.Invoke();
    }
}
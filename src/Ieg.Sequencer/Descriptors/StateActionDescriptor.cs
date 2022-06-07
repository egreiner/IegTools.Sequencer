namespace Ieg.Sequencer.Descriptors;


/// <summary>
/// Invokes an specified action when the sequence is in the specified state
/// </summary>
public class StateActionDescriptor : DescriptorBase
{
    public StateActionDescriptor(string state, Action action)
    {
        State  = state;
        Action = action;
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

    /// <summary>
    /// Returns true if the sequence is in the specified state
    /// </summary>
    /// <param name="sequence"></param>
    public override bool ValidateAction(ISequence sequence) => State == sequence.CurrentState;

    /// <summary>
    /// Invokes the specified action
    /// </summary>
    /// <param name="sequence"></param>
    public override void ExecuteAction(ISequence sequence)
    {
        Action?.Invoke();
    }
}
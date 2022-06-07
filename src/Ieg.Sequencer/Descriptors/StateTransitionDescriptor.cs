namespace Ieg.Sequencer.Descriptors;

/// <summary>
/// Moves the sequence from the current state to the next state
/// when the constraint is met
/// and invokes the specified action
/// </summary>
public class StateTransitionDescriptor : DescriptorBase
{
    public StateTransitionDescriptor(string currentState, string nextState, Func<bool> constraint, Action action)
    {
        CurrentState = currentState;
        NextState    = nextState;
        Constraint   = constraint;
        Action       = action;
    }

    /// <summary>
    /// The current state the sequence must met
    /// </summary>
    public string CurrentState   { get; }

    /// <summary>
    /// The next state the sequence will be in when the constraint is met
    /// </summary>
    public string NextState      { get; }

    /// <summary>
    /// The constraint
    /// </summary>
    public Func<bool> Constraint { get; }

    /// <summary>
    /// The action that will be invoked when the state transition will be executed
    /// </summary>
    public Action Action         { get; }

    
    public override string ToString() =>
        $"{CurrentState}->{NextState} (Transition)";


    /// <summary>
    /// Returns true if the sequence met the specified state and the constraint is fulfilled
    /// </summary>
    /// <param name="sequence">The sequence</param>
    public override bool ValidateAction(ISequence sequence) =>
        CurrentState == sequence.CurrentState && (Constraint?.Invoke() ?? true);


    /// <summary>
    /// Invokes the action if the validation is fulfilled
    /// </summary>
    /// <param name="sequence"></param>
    public override void ExecuteAction(ISequence sequence)
    {
        sequence.SetState(NextState);
        Action?.Invoke();
    }
}
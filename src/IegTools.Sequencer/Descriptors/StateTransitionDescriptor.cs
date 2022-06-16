namespace IegTools.Sequencer.Descriptors;

/// <summary>
/// Moves the sequence from the current state to the next state
/// when the constraint is met
/// and invokes the specified action
/// </summary>
public class StateTransitionDescriptor : DescriptorBase
{
    public StateTransitionDescriptor(string fromState, string toState, Func<bool> constraint, Action action)
    {
        FromState  = fromState;
        ToState    = toState;
        Constraint = constraint;
        Action     = action;
    }

    /// <summary>
    /// The state from which the transition should be made
    /// </summary>
    public string FromState   { get; }

    /// <summary>
    /// The state to which the transition should be made
    /// </summary>
    public string ToState      { get; }

    /// <summary>
    /// The constraint that should be met to make the transition
    /// </summary>
    public Func<bool> Constraint { get; }

    /// <summary>
    /// The action that will be invoked when the state transition will be executed
    /// </summary>
    public Action Action         { get; }

    
    public override string ToString() =>
        $"{FromState}->{ToState} (Transition)";


    /// <summary>
    /// Returns true if the sequence met the specified state and the constraint is fulfilled
    /// </summary>
    /// <param name="sequence">The sequence</param>
    public override bool ValidateAction(ISequence sequence) =>
        FromState == sequence.CurrentState && (Constraint?.Invoke() ?? true);


    /// <summary>
    /// Invokes the action if the validation is fulfilled
    /// </summary>
    /// <param name="sequence">The sequence</param>
    public override void ExecuteAction(ISequence sequence)
    {
        sequence.SetState(ToState);
        Action?.Invoke();
    }
}
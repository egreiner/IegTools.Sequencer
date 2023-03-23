namespace IegTools.Sequencer.Descriptors;

using System.Linq;

/// <summary>
/// Transfers the sequence from the current state to the next state
/// when the constraint is met
/// and invokes the specified action
/// </summary>
public class AnyStateTransitionDescriptor : DescriptorBase
{
    public AnyStateTransitionDescriptor(string[] fromStates, string toState, Func<bool> constraint, Action action)
    {
        FromStates  = fromStates;
        ToState    = toState;
        Constraint = constraint;
        Action     = action;
    }


    /// <summary>
    /// The state from which the transition should be made
    /// </summary>
    public string[] FromStates   { get; }

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
        $"{FromStates}->{ToState} (Transition)";


    /// <inheritdoc />
    public override bool IsRegisteredState(string state) =>
        state == ToState || FromStates.Contains(state);

    /// <summary>
    /// Returns true if the sequence met the specified state and the constraint is fulfilled
    /// </summary>
    /// <param name="sequence">The sequence</param>
    public override bool ValidateAction(ISequence sequence) =>
        sequence.CurrentState != ToState && FromStates.Contains(sequence.CurrentState) && (Constraint?.Invoke() ?? true);


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
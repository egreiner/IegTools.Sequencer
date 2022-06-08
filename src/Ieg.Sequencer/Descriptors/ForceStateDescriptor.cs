namespace Ieg.Sequencer.Descriptors;

/// <summary>
/// Forces the sequence to the specified state if the constraint is fulfilled
/// </summary>
public class ForceStateDescriptor : DescriptorBase
{
    public ForceStateDescriptor(string state, Func<bool> constraint)
    {
        State      = state;
        Constraint = constraint;
        ResumeSequence = false;
    }


    /// <summary>
    /// The state the sequence should be forced to
    /// </summary>
    public string State          { get; }
    
    /// <summary>
    /// This constraint must be fulfilled that the sequence will be set to the specified state
    /// </summary>
    public Func<bool> Constraint { get; }

    
    public override string ToString() => $"{State} (Forced)";

    /// <summary>
    /// This descriptor is not dependent on the current sequence state.
    /// It depends on the constraint only.
    /// </summary>
    /// <param name="sequence">The sequence</param>
    public override bool ValidateAction(ISequence sequence) => Constraint?.Invoke() ?? false;

    /// <summary>
    /// The sequence will be set to the specified state
    /// </summary>
    /// <param name="sequence">The sequence</param>
    public override void ExecuteAction(ISequence sequence)
    {
        sequence.SetState(State);
    }
}
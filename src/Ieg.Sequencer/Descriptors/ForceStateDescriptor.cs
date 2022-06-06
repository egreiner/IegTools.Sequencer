namespace Ieg.Sequencer.Descriptors;

public class ForceStateDescriptor : SequenceDescriptor
{
    public ForceStateDescriptor(string state, Func<bool> constraint)
    {
        State      = state;
        Constraint = constraint;
    }

    
    public string State          { get; }
    public Func<bool> Constraint { get; }

    
    public override string ToString() => $"{State} (Forced)";

    /// <summary>
    /// This descriptor is not dependent on the current sequence state.
    /// It depends on the constraint only.
    /// </summary>
    /// <param name="sequence"></param>
    public override bool ValidateAction(ISequence sequence) => Constraint?.Invoke() ?? false;

    public override void ExecuteAction(ISequence sequence)
    {
        sequence.SetState(State);
    }
}
namespace Ieg.Sequencer.Descriptors;

public class ForceStateDescriptor : SequenceDescriptor
{
    public ForceStateDescriptor(string state, Func<bool> constraint)
    {
        this.State      = state;
        this.Constraint = constraint;
    }

    
    public string State          { get; init; }
    public Func<bool> Constraint { get; init; }

    
    public override string ToString() => $"{State} (Forced)";
}
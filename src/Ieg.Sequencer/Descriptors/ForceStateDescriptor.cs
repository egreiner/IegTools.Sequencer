namespace Ieg.Sequencer.Descriptors;

public class ForceStateDescriptor : SequenceDescriptor
{
    public ForceStateDescriptor(string State, Func<bool> Constraint)
    {
        this.State = State;
        this.Constraint = Constraint;
    }

    public override string ToString() => $"{State} (Forced)";
    public string State { get; init; }
    public Func<bool> Constraint { get; init; }

    public void Deconstruct(out string State, out Func<bool> Constraint)
    {
        State = this.State;
        Constraint = this.Constraint;
    }
}
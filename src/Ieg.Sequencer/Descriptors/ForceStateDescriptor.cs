namespace Ieg.Sequencer.Descriptors;

public record ForceStateDescriptor(string State, Func<bool> Constraint) : SequenceDescriptor
{
    public override string ToString() => $"{State} (Forced)";
}
namespace Ieg.Sequencer;

using System.Collections.Generic;

public class SequenceConfiguration
{
    public List<SequenceDescriptor> Descriptors { get; } = new();

    public string InitialState { get; set; }
}
namespace Ieg.Sequencer;

using System.Collections.Generic;
using Descriptors;

public class SequenceConfiguration
{
    public List<SequenceDescriptor> Descriptors { get; } = new();

    public string InitialState { get; set; }
}
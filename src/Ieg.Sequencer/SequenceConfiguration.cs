namespace Ieg.Sequencer;

using System.Collections.Generic;
using Descriptors;

public class SequenceConfiguration
{
    public bool DisableValidation { get; set; }

    public string InitialState { get; set; }
    
    public List<SequenceDescriptor> Descriptors { get; } = new();
}
namespace Ieg.Sequencer;

using System.Collections.Generic;
using Descriptors;

public class SequenceConfiguration
{
    public char IgnoreTag { get; set; } = '!';
    public char InitialStateTag { get; set; } = '>';

    public bool     DisableValidation          { get; set; }
    public string[] DisableValidationForStates { get; set; }

    public string InitialState { get; set; }
    
    public List<SequenceDescriptor> Descriptors { get; } = new();
}
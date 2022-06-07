namespace Ieg.Sequencer;

using System.Collections.Generic;
using Descriptors;

public class SequenceConfiguration
{
    /// <summary>
    /// Prefix states with the IgnoreTag, does tag states as dead-end-states (the validator will not check for dead ends)
    /// </summary>
    public char IgnoreTag { get; set; } = '!';

    /// <summary>
    /// Prefix states with the InitialStateTag, does tag states for the sequence to start from
    /// </summary>
    public char InitialStateTag { get; set; } = '>';

    /// <summary>
    /// The complete validation will be disabled
    /// </summary>
    public bool     DisableValidation          { get; set; }

    /// <summary>
    /// Does tell the validator to not check this states
    /// </summary>
    public string[] DisableValidationForStates { get; set; }

    /// <summary>
    /// The state the sequence will start from
    /// </summary>
    public string InitialState { get; set; }
    
    /// <summary>
    /// The sequence-descriptors that describe how the sequence is supposed to work
    /// </summary>
    public List<IDescriptor> Descriptors { get; } = new();
}
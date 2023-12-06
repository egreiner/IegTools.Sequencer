namespace IegTools.Sequencer;

using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Handler;
using Validation;

/// <summary>
/// The sequence configuration
/// </summary>
public class SequenceConfiguration
{
    /// <summary>
    /// The logger that can be used for logging
    /// </summary>
    public ILogger? Logger { get; set; }

    /// <summary>
    /// Prefix states with the IgnoreTag, does tag states as dead-end-states (the validator will not check for dead ends)
    /// </summary>
    public char IgnoreTag { get; set; } = '!';

    /// <summary>
    /// Prefix states with the InitialStateTag, does tag states for the sequence to start from
    /// </summary>
    public char InitialStateTag { get; set; } = '>';

    /// <summary>
    /// Is an empty sequence.
    /// On run nothings happens.
    /// No state changes.
    /// Validation is disabled.
    /// </summary>
    public bool IsEmpty { get; set; }

    /// <summary>
    /// The complete validation will be disabled
    /// </summary>
    public bool DisableValidation { get; set; }

    /// <summary>
    /// Does tell the validator to not check this states
    /// </summary>
    public string[] DisableValidationForStates { get; set; }

    /// <summary>
    /// The state the sequence will start from
    /// </summary>
    public string InitialState { get; set; }

    /// <summary>
    /// The sequence-handler that describe how the sequence is supposed to work
    /// </summary>
    public List<IHandler> Handler { get; } = new();

    /// <summary>
    /// All validators
    /// </summary>
    public IList<IHandlerValidator> Validators { get; set; } = new List<IHandlerValidator>();
}
﻿namespace IegTools.Sequencer;

using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Handler;
using Validation;

/// <summary>
/// The sequence configuration
/// </summary>
public class SequenceConfiguration
{
    // TODO the logger-adapter will be created in builder and injected in the handler
    // /// <summary>
    // /// The logger that can be used for logging
    // /// </summary>
    // public ILoggerAdapter LoggerAdapter { get; set; }


    /// <summary>
    /// TODO move to logger-adapter
    /// The logger that can be used for logging
    /// </summary>
    public ILogger Logger    { get; set; }

    /// <summary>
    /// TODO move to logger-adapter
    /// The logger-scope
    /// </summary>
    public Func<IDisposable> LoggerScope { get; set; }

    /// <summary>
    /// TODO move to logger-adapter
    /// The EventId for logging
    /// </summary>
    public EventId EventId { get; set; }



    /// <summary>
    /// The complete validation will be disabled
    /// </summary>
    public bool DisableValidation { get; set; }

    /// <summary>
    /// Does tell the validator to not check this states
    /// </summary>
    public string[] DisableValidationForStates { get; set; }



    /// <summary>
    /// Is an empty sequence.
    /// On run nothings happens.
    /// No state changes.
    /// Validation is disabled.
    /// </summary>
    public bool IsEmpty { get; set; }



    /// <summary>
    /// Prefix states with the IgnoreTag, does tag states as dead-end-states (the validator will not check for dead ends)
    /// </summary>
    public char IgnoreTag { get; set; } = '!';

    /// <summary>
    /// Prefix states with the InitialStateTag, does tag states for the sequence to start from
    /// </summary>
    public char InitialStateTag { get; set; } = '>';

    /// <summary>
    /// The state the sequence will start from
    /// </summary>
    public string InitialState { get; set; }
}
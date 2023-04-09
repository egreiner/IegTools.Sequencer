namespace IegTools.Sequencer;

using System.Collections.Generic;
using Handler;
using Validation;

/// <summary>
/// The sequence configuration
/// </summary>
public class SequenceConfiguration
{
    /// <summary>
    /// Prefix statuses with the IgnoreTag, does tag statuses as dead-end-statuses (the validator will not check for dead ends)
    /// </summary>
    public char IgnoreTag { get; set; } = '!';

    /// <summary>
    /// Prefix statuses with the InitialStateTag, does tag statuses for the sequence to start from
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
    /// Does tell the validator to not check this statuses
    /// </summary>
    public string[] DisableValidationForStatuses { get; set; }

    /// <summary>
    /// The state the sequence will start from
    /// </summary>
    public string InitialState { get; set; }

    /// <summary>
    /// The sequence-rules that describe how the sequence is supposed to work
    /// </summary>
    public List<IHandler> Rules { get; } = new();

    /// <summary>
    /// All rule validators
    /// </summary>
    public IList<ISequenceRuleValidator> RuleValidators { get; set; } = new List<ISequenceRuleValidator>();
}
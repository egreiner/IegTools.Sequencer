namespace IegTools.Sequencer;

/// <summary>
/// The sequence configuration
/// </summary>
public class SequenceConfiguration
{
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
    /// On run nothing happens.
    /// No state changes.
    /// Validation is disabled.
    /// </summary>
    public bool IsEmpty { get; set; }



    /// <summary>
    /// Prefix states with the IgnoreTag, does tag states as dead-end-states (the validator will not check for dead ends)
    /// </summary>
    //[Obsolete("TODO this will be removed in the next major version (v4.0)")]
    public char IgnoreTag { get; set; } = '!';

    /// <summary>
    /// Prefix states with the InitialStateTag, does tag states for the sequence to start from
    /// </summary>
    //[Obsolete("TODO this will be removed in the next major version (v4.0)")]
    public char InitialStateTag { get; set; } = '>';

    /// <summary>
    /// The state the sequence will start from
    /// </summary>
    public string InitialState { get; set; }
}
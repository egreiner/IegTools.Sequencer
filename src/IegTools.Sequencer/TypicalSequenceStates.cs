namespace IegTools.Sequencer;

/// <summary>
/// Defines typical sequence states.
/// The reason why this is not an enum is easy to inherit and extend.
/// Build your own custom states and inherit from this class if the states are used internally only.
/// Better use your own Enum if the states are used in public interfaces.
/// </summary>
public class TypicalSequenceStates
{
    /// <summary>
    /// State Initializing
    /// </summary>
    public readonly string Initializing = "Initializing";

    /// <summary>
    /// State Initialized
    /// </summary>
    public readonly string Initialized  = "Initialized";

    /// <summary>
    /// State Enabled
    /// </summary>
    public readonly string Enabled = "Enabled";

    /// <summary>
    /// State Disabled
    /// </summary>
    public readonly string Disabled = "Disabled";

    /// <summary>
    /// State On
    /// </summary>
    public readonly string On  = "On";

    /// <summary>
    /// State Off
    /// </summary>
    public readonly string Off = "Off";

    /// <summary>
    /// State WaitOn
    /// </summary>
    public readonly string WaitOn  = "WaitOn";

    /// <summary>
    /// State WaitOff
    /// </summary>
    public readonly string WaitOff = "WaitOff";

    /// <summary>
    /// State Activated
    /// </summary>
    public readonly string Activated = "Activated";

    /// <summary>
    /// State Deactivated
    /// </summary>
    public readonly string Deactivated = "Deactivated";

    /// <summary>
    /// State Paused
    /// </summary>
    public readonly string Paused = "Paused";

    /// <summary>
    /// State Paused
    /// </summary>
    public readonly string Resumed = "Resumed";

    /// <summary>
    /// State Prepared
    /// </summary>
    public readonly string Prepared  = "Prepared";
}
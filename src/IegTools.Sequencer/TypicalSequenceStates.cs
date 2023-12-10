#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace IegTools.Sequencer;

/// <summary>
/// The TypicalSequenceStates class defines a set of standard states for sequences.
/// Designed as a class rather than an enum for greater flexibility, it allows easy inheritance and extension.
/// Use this class to define internal sequence states within your application, offering the ability to add custom states as needed.
/// For public interfaces, where a fixed set of states is preferable, consider defining an enum instead.
/// </summary>
public class TypicalSequenceStates
{
    public readonly string Initializing = "Initializing";
    public readonly string Initialized  = "Initialized";

    public readonly string Enabled  = "Enabled";
    public readonly string Disabled = "Disabled";

    public readonly string On  = "On";
    public readonly string Off = "Off";

    public readonly string Wait    = "Wait";
    public readonly string WaitOn  = "WaitOn";
    public readonly string WaitOff = "WaitOff";

    public readonly string Activated   = "Activated";
    public readonly string Deactivated = "Deactivated";

    public readonly string Paused  = "Paused";
    public readonly string Resumed = "Resumed";

    public readonly string Preparing = "Preparing";
    public readonly string Prepared  = "Prepared";
}
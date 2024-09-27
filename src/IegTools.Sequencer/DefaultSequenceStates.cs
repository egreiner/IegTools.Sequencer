#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace IegTools.Sequencer;

/// <summary>
/// The DefaultSequenceStates class defines a set of standard states for sequences.
/// Designed as a 'Named Constants Class' rather than an enum for greater flexibility (inherit for custom states).
/// Use this class to define internal sequence states within your application, offering the ability to add custom states as needed.
/// </summary>
public class DefaultSequenceStates
{
    public readonly string Initializing = nameof(Initializing);
    public readonly string Initialized  = nameof(Initialized);

    public readonly string Enabled  = nameof(Enabled);
    public readonly string Disabled = nameof(Disabled);

    public readonly string On  = nameof(On);
    public readonly string Off = nameof(Off);

    public readonly string Activated   = nameof(Activated);
    public readonly string Deactivated = nameof(Deactivated);

    public readonly string Paused  = nameof(Paused);
    public readonly string Resumed = nameof(Resumed);

    public readonly string Pending   = nameof(Pending);
    public readonly string Preparing = nameof(Preparing);
    public readonly string Prepared  = nameof(Prepared);
}
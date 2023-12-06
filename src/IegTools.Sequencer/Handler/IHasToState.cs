namespace IegTools.Sequencer.Handler;

/// <summary>
/// Interface for a handler that has a state to which the transition should be made
/// </summary>
public interface IHasToState
{
    /// <summary>
    /// The state to which the transition should be made
    /// </summary>
    string ToState { get; }
}
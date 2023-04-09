namespace IegTools.Sequencer.Handler;

public interface IHasToState
{
    /// <summary>
    /// The state to which the transition should be made
    /// </summary>
    string ToState { get; }
}
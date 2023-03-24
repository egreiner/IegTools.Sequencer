namespace IegTools.Sequencer.Descriptors;

public interface IHasFromState
{
    /// <summary>
    /// The state from which the transition should be made
    /// </summary>
    string FromState { get; }
}
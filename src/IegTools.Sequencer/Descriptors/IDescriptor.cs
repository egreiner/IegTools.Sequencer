namespace IegTools.Sequencer.Descriptors;

public interface IDescriptor
{
    /// <summary>
    /// Standard is that the sequence should continue to run after a action is executed
    /// </summary>
    bool ResumeSequence { get; set; }

    /// <summary>
    /// Returns true if the action is allowed to be executed
    /// </summary>
    /// <param name="sequence">The sequence</param>
    bool ValidateAction(ISequence sequence);

    /// <summary>
    /// Invokes the specified action and gives the possibility to manipulate the sequence
    /// </summary>
    /// <param name="sequence">The sequence</param>
    void ExecuteAction(ISequence sequence);
    
    /// <summary>
    /// Validates and invokes the action.
    /// </summary>
    /// <param name="sequence">The sequence</param>
    bool ExecuteIfValid(ISequence sequence);
    
    /// <summary>
    /// Returns true if the queried state is registered in the descriptor.
    /// </summary>
    /// <param name="state">The state</param>
    bool IsRegisteredState(string state);
}
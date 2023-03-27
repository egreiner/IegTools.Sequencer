namespace IegTools.Sequencer.Descriptors;

public interface IDescriptor
{
    /// <summary>
    /// The Descriptor Id
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Standard is that the sequence should continue to run after a action is executed
    /// </summary>
    bool ResumeSequence { get; set; }

    /////// <summary>
    /////// All states that should be validated from the SequenceValidator
    /////// </summary>
    ////HashSet<string> ValidationTargetStates { get; set; }

    /// <summary>
    /// The Condition that should be met to make the transition
    /// </summary>
    Func<bool> Condition { get; set; }

    /// <summary>
    /// The action that will be invoked when the state transition will be executed
    /// </summary>
    Action Action { get; set; }


    /// <summary>
    /// Returns true if all conditions are fulfilled and the action is allowed to be executed
    /// </summary>
    /// <param name="sequence">The sequence</param>
    bool IsConditionFulfilled(ISequence sequence);

    /// <summary>
    /// Executes the specified action and enables the adjustment of the sequence state. 
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
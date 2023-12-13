namespace IegTools.Sequencer.Handler;

/// <summary>
/// The handler interface.
/// </summary>
public interface IHandler
{
    /// <summary>
    /// The handlers name
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The description of the transition
    /// </summary>
    string Description { get; }

    /// <summary>
    /// The Sequence-Configuration
    /// </summary>
    SequenceConfiguration Configuration { get; set; }

    /// <summary>
    /// Standard is that the sequence should continue to run after a action is executed
    /// </summary>
    bool ResumeSequence { get; set; }

    /// <summary>
    /// The Condition that should be met to make the transition
    /// </summary>
    Func<bool> Condition { get; set; }

    /// <summary>
    /// The action that will be invoked when the state transition will be executed
    /// </summary>
    Action Action { get; set; }

    /// <summary>
    /// The last time the handler was executed
    /// </summary>
    DateTime LastExecutedAt { get; }


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
    /// Returns true if the queried state is registered in the handler.
    /// </summary>
    /// <param name="state">The state</param>
    bool IsRegisteredState(string state);

    /// <summary>
    /// Sets a time span that defines the time that the action is allowed to be executed only once
    /// </summary>
    /// <param name="timeSpan">The timespan in which the execution is allowed only once</param>
    void AllowOnlyOnceIn(TimeSpan timeSpan);
}
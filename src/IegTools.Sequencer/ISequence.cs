namespace IegTools.Sequencer;

using System.Diagnostics;
using System.Threading.Tasks;
using Logging;

/// <summary>
/// Interface for a sequence
/// </summary>
public interface ISequence
{
    /// <summary>
    /// The debug logger
    /// </summary>
    ILoggerAdapter Logger { get; set; }

    /// <summary>
    /// The sequence-configuration
    /// </summary>
    SequenceConfiguration Configuration { get; }

    /// <summary>
    /// The sequence-data
    /// </summary>
    SequenceData Data { get; }


    /// <summary>
    /// The current state of the sequence
    /// </summary>
    string CurrentState { get; }


    /// <summary>
    /// The last state of the sequence (previous state)
    /// </summary>
    (string Value, TimeSpan Duration) LastState { get; }

    /// <summary>
    /// A builtin stopwatch
    /// </summary>
    Stopwatch Stopwatch { get; }


    /// <summary>
    /// Returns true if the sequence.CurrentState is in the specified state.
    /// </summary>
    /// <param name="state">The state that is asked for.</param>
    bool HasCurrentState(string state);

    /// <summary>
    /// Returns true if the sequence.CurrentState is in one of the specified states.
    /// </summary>
    /// <param name="states">The states that are asked for.</param>
    bool HasAnyCurrentState(params string[] states);

    /// <summary>
    /// Returns true if the queried state is registered in the sequence-configuration.
    /// </summary>
    /// <param name="state">The state</param>
    bool IsRegisteredState(string state);


    /// <summary>
    /// Run the sequence
    /// </summary>
    ISequence Run();

    /// <summary>
    /// Runs the sequence asynchronous
    /// </summary>
    Task<ISequence> RunAsync();


    /// <summary>
    /// Set the sequence-configuration
    /// </summary>
    ISequence SetConfiguration(SequenceConfiguration configuration, SequenceData data);


    /// <summary>
    /// If the constraint is fulfilled the CurrentState will be set to the state immediately
    /// and the execution of the sequence will continue.
    /// </summary>
    /// <param name="state">The state that should be set</param>
    /// <param name="constraint">The constraint that must be fulfilled that the sequence is set to the defined state</param>
    ISequence SetState(string state, Func<bool> constraint);

    /// <summary>
    /// CurrentState will be set to the state immediately and unconditional.
    /// The execution of the sequence will continue.
    /// </summary>
    /// <param name="state">The state that will be set</param>
    ISequence SetState(string state);
}
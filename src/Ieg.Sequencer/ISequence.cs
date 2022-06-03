namespace Ieg.Sequencer;

using System.Diagnostics;

public interface ISequence
{
    /// <summary>
    /// The current state of the sequence.
    /// </summary>
    string CurrentState { get; }

    /// <summary>
    /// A builtin stopwatch.
    /// </summary>
    Stopwatch Stopwatch { get; }
        
    /// <summary>
    /// Run the sequence.
    /// </summary>
    ISequence Run();

    /// <summary>
    /// CurrentState will be set to the state immediately and unconditional.
    /// The execution of the sequence will continue.
    /// </summary>
    /// <param name="state">The state that will be set.</param>
    ISequence SetState(string state);

    /// <summary>
    /// If the constraint is fulfilled the CurrentState will be set to the state immediately
    /// and the execution of the sequence will continue.
    /// </summary>
    /// <param name="state">The state that should be set.</param>
    /// <param name="constraint">The constraint that must be fulfilled that the sequence is set to the defined state.</param>
    ISequence SetState(string state, Func<bool> constraint);
}
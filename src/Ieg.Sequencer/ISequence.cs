namespace Ieg.Sequencer;

using System;
using System.Diagnostics;

public interface ISequence
{
    string CurrentState { get; }

    Stopwatch Stopwatch { get; }
        
    ISequence Run();

    /// <summary>
    /// CurrentState will be set to the state immediately and unconditional.
    /// and the execution of the sequence will continue.
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
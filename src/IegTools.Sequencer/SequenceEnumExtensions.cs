namespace IegTools.Sequencer;

using System.Linq;

public static class SequenceEnumExtensions
{
    /// <summary>
    /// Returns true if the sequence.CurrentState is in the specified state.
    /// </summary>
    /// <param name="sequence">The sequence.</param>
    /// <param name="state">The state that is asked for.</param>
    public static bool HasCurrentState<T>(this ISequence sequence, T state)
        where T : Enum =>
        sequence.HasCurrentState(state.ToString());

    /// <summary>
    /// Returns true if the sequence.CurrentState is in one of the specified states.
    /// </summary>
    /// <param name="sequence">The sequence.</param>
    /// <param name="states">The states that are asked for.</param>
    public static bool HasAnyCurrentState<T>(this ISequence sequence, params T[] states)
        where T : Enum =>
        states.Any(sequence.HasCurrentState);

    /// <summary>
    /// Returns true if the queried state is registered in the sequence-configuration.
    /// </summary>
    /// <param name="sequence">The sequence.</param>
    /// <param name="state">The state</param>
    public static bool IsRegisteredState<T>(this ISequence sequence, T state) =>
        sequence.IsRegisteredState(state.ToString());


    /// <summary>
    /// CurrentState will be set to the state immediately and unconditional.
    /// and the execution of the sequence will continue.
    /// </summary>
    /// <param name="sequence">The sequence.</param>
    /// <param name="state">The state that will be set.</param>
    public static ISequence SetState<T>(this ISequence sequence, T state)
        where T : Enum =>
        sequence.SetState(state.ToString());
    
    /// <summary>
    /// If the constraint is fulfilled the CurrentState will be set to the state immediately
    /// and the execution of the sequence will continue.
    /// </summary>
    /// <param name="sequence">The sequence.</param>
    /// <param name="state">The state that should be set.</param>
    /// <param name="constraint">The constraint that must be fulfilled that the sequence is set to the defined state.</param>
    public static ISequence SetState<T>(this ISequence sequence, T state, Func<bool> constraint)
        where T : Enum =>
        sequence.SetState(state.ToString(), constraint);
}
namespace IegTools.Sequencer;

public static class SequenceEnumExtensions
{
    /// <summary>
    /// CurrentState will be set to the state immediately and unconditional.
    /// and the execution of the sequence will continue.
    /// </summary>
    /// <param name="sequence">The sequence.</param>
    /// <param name="state">The state that will be set.</param>
    public static ISequence SetState<T>(this ISequence sequence, T state)
        where T: Enum =>
        sequence.SetState(state.ToString());

    /// <summary>
    /// If the constraint is fulfilled the CurrentState will be set to the state immediately
    /// and the execution of the sequence will continue.
    /// </summary>
    /// <param name="sequence">The sequence.</param>
    /// <param name="state">The state that should be set.</param>
    /// <param name="constraint">The constraint that must be fulfilled that the sequence is set to the defined state.</param>
    public static ISequence SetState<T>(this ISequence sequence, T state, Func<bool> constraint)
        where T: Enum =>
        sequence.SetState(state.ToString(), constraint);
}
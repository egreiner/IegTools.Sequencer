namespace IegTools.Sequencer;

using Descriptors;

public static class SequenceBuilderEnumExtensions
{
    /// <summary>
    /// Sets the initial state
    /// </summary>
    /// <param name="builder">The sequence-builder</param>
    /// <param name="initialState">The initial state</param>
    /// <returns></returns>
    public static ISequenceBuilder SetInitialState<T>(this ISequenceBuilder builder, T initialState)   
        where T: Enum =>
        builder.SetInitialState(initialState.ToString());

    /// <summary>
    /// Adds a 'state to state'-transition.
    /// The state transition will be executed if the constraint is complied.
    /// The action will be executed just once, at the moment when the constraint is complied.
    /// </summary>
    /// <param name="builder">The sequence-builder</param>
    /// <param name="currentState">The current state</param>
    /// <param name="nextState">The next state</param>
    /// <param name="constraint">The constraint</param>
    /// <param name="action">The action that should be executed.</param>
    public static ISequenceBuilder AddTransition<T>(
        this ISequenceBuilder builder, T currentState, T nextState, Func<bool> constraint, Action action = null) 
        where T: Enum =>
        builder.AddDescriptor(new StateTransitionDescriptor(currentState.ToString(), nextState.ToString(), constraint, action));

    /// <summary>
    /// Adds a state action that should be executed during the state is active.
    /// Internal it's handled like a StateTransition...
    /// </summary>
    /// <param name="builder">The sequence-builder</param>
    /// <param name="currentState">State of the current.</param>
    /// <param name="action">The action.</param>
    public static ISequenceBuilder AddStateAction<T>(this ISequenceBuilder builder, T currentState, Action action)
        where T: Enum =>
        builder.AddTransition(currentState, currentState, () => true, action);

    /// <summary>
    /// Adds a ForceStateDescriptor to the sequence-descriptors.
    /// If the constraint is fulfilled on execution the CurrentState will be set to the state
    /// and further execution of the sequence will be prevented.
    /// </summary>
    /// <param name="builder">The sequence-builder</param>
    /// <param name="state">The state that should be forced.</param>
    /// <param name="constraint">The constraint that must be fulfilled that the sequence is forced to the defined state.</param>
    public static ISequenceBuilder AddForceState<T>(this ISequenceBuilder builder, T state, Func<bool> constraint)
        where T: Enum =>
        builder.AddDescriptor(new ForceStateDescriptor(state.ToString(), constraint));
}
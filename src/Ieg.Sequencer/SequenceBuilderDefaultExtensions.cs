namespace Ieg.Sequencer;

using Descriptors;

public static class SequenceBuilderDefaultExtensions
{
    /// <summary>
    /// Adds a 'state to state'-transition.
    /// The state transition will be executed if the constraint is complied.
    /// The action will be executed just once, at the moment when the constraint is complied.
    /// </summary>
    /// <param name="builder">The sequence-builder</param>
    /// <param name="currentState">The current state.</param>
    /// <param name="nextState">The next state.</param>
    /// <param name="constraint">The constraint.</param>
    /// <param name="action">The action that should be executed.</param>
    public static ISequenceBuilder AddTransition(this ISequenceBuilder builder, string currentState, string nextState, Func<bool> constraint, Action action = null)
    {
        builder.AddInitialStates(currentState, nextState);
        return builder.AddDescriptor(
            new StateTransitionDescriptor(currentState, nextState, constraint, action));
    }


    /// <summary>
    /// Adds a state action that should be executed during the state is active.
    /// Internal it's handled like a StateTransition...
    /// </summary>
    /// <param name="builder">The sequence-builder</param>
    /// <param name="state">The state where the action should be invoked</param>
    /// <param name="action">The action.</param>
    public static ISequenceBuilder AddStateAction(this ISequenceBuilder builder, string state, Action action)
    {
        builder.AddInitialStates(state);
        return builder.AddDescriptor(new StateActionDescriptor(state, action));
    }

    /// <summary>
    /// Adds a ForceStateDescriptor to the sequence-descriptors.
    /// If the constraint is fulfilled on execution the CurrentState will be set to the state
    /// and further execution of the sequence will be prevented.
    /// </summary>
    /// <param name="builder">The sequence-builder</param>
    /// <param name="state">The state that should be forced.</param>
    /// <param name="constraint">The constraint that must be fulfilled that the sequence is forced to the defined state.</param>
    public static ISequenceBuilder AddForceState(this ISequenceBuilder builder, string state, Func<bool> constraint)
    {
        builder.AddInitialStates(state);
        return builder.AddDescriptor(new ForceStateDescriptor(state, constraint));
    }


    private static void AddInitialStates(this ISequenceBuilder builder, params string[] states)
    {
        foreach (var state in states)
        {
            if (state.StartsWith(builder.InitialStateTag()))
                builder.SetInitialState(state);
        }
    }

    private static string InitialStateTag(this ISequenceBuilder builder) =>
        builder.Configuration.InitialStateTag.ToString();
}
// ReSharper disable once CheckNamespace, should be reachable from root-namespace (no additional usings are needed)
namespace IegTools.Sequencer;

using System.Linq;

/// <summary>
/// Default Enum-Extensions für the SequenceBuilder.
/// Forward all calls to <see cref="SequenceBuilderAddHandlerStringStateExtensions"/>
/// </summary>
public static class SequenceBuilderAddHandlerEnumStateExtensions
{
    /// <summary>
    /// Adds a 'state to state'-transition.
    /// The state transition will be executed if the condition is complied.
    /// The action will be executed just once, at the moment when the condition is complied.
    /// </summary>
    /// <param name="builder">The sequence-builder</param>
    /// <param name="currentState">The current state</param>
    /// <param name="nextState">The next state</param>
    /// <param name="condition">The condition</param>
    /// <param name="action">The action that should be executed.</param>
    public static ISequenceBuilder AddTransition<T>(
        this ISequenceBuilder builder, T currentState, T nextState, Func<bool> condition, Action action = null)
        where T : Enum =>
        builder.AddTransition(currentState.ToString(), nextState.ToString(), condition, action);

    /// <summary>
    /// Adds a 'state to state'-transition.
    /// The state transition will be executed if the condition is complied.
    /// The action will be executed just once, at the moment when the condition is complied.
    /// </summary>
    /// <param name="builder">The sequence-builder</param>
    /// <param name="description">The transition description (for debugging or just to describe what is it for)</param>
    /// <param name="currentState">The current state</param>
    /// <param name="nextState">The next state</param>
    /// <param name="condition">The condition</param>
    /// <param name="action">The action that should be executed.</param>
    public static ISequenceBuilder AddTransition<T>(
        this ISequenceBuilder builder, string description, T currentState, T nextState, Func<bool> condition, Action action = null)
        where T : Enum =>
        builder.AddTransition(description, currentState.ToString(), nextState.ToString(), condition, action);



    /// <summary>
    /// Adds a 'state_s_ to state'-transition.
    /// The state transition will be executed if
    /// the CurrentState-string contains a substring of the currentStateContains
    /// (eg1. CurrentState 'ActivatedByApi' contains 'Activated')
    /// (eg2. CurrentState 'ActivatedByApi' contains 'ByApi')
    /// (eg3. CurrentState 'ActivatedByApi' contains 'tedBy')
    /// and the condition is complied.
    /// The action will be executed just once, at the moment when the condition is complied.
    /// </summary>
    /// <param name="builder">The sequence-builder</param>
    /// <param name="currentStateContains">Does current-state contains this substring?</param>
    /// <param name="nextState">The next state.</param>
    /// <param name="condition">The condition.</param>
    /// <param name="action">The action that should be executed.</param>
    public static ISequenceBuilder AddContainsTransition<T>(
        this ISequenceBuilder builder, string currentStateContains, T nextState, Func<bool> condition, Action action = null)
        where T : Enum =>
        builder.AddContainsTransition(currentStateContains, nextState.ToString(), condition, action);

    /// <summary>
    /// Adds a 'state_s_ to state'-transition.
    /// The state transition will be executed if
    /// the CurrentState-string contains a substring of the currentStateContains
    /// (eg1. CurrentState 'ActivatedByApi' contains 'Activated')
    /// (eg2. CurrentState 'ActivatedByApi' contains 'ByApi')
    /// (eg3. CurrentState 'ActivatedByApi' contains 'tedBy')
    /// and the condition is complied.
    /// The action will be executed just once, at the moment when the condition is complied.
    /// </summary>
    /// <param name="builder">The sequence-builder</param>
    /// <param name="description">The transition description (for debugging or just to describe what is it for)</param>
    /// <param name="currentStateContains">Does current-state contains this substring?</param>
    /// <param name="nextState">The next state.</param>
    /// <param name="condition">The condition.</param>
    /// <param name="action">The action that should be executed.</param>
    public static ISequenceBuilder AddContainsTransition<T>(
        this ISequenceBuilder builder, string description, string currentStateContains, T nextState, Func<bool> condition, Action action = null)
        where T : Enum =>
        builder.AddContainsTransition(description, currentStateContains, nextState.ToString(), condition, action);




    /// <summary>
    /// Adds a 'state to state'-transition.
    /// The state transition will be executed if the condition is complied.
    /// The action will be executed just once, at the moment when the condition is complied.
    /// </summary>
    /// <param name="builder">The sequence-builder</param>
    /// <param name="compareStates">The state(s) that will be compared with the current state.</param>
    /// <param name="nextState">The next state</param>
    /// <param name="condition">The condition</param>
    /// <param name="action">The action that should be executed.</param>
    public static ISequenceBuilder AddAnyTransition<T>(
        this ISequenceBuilder builder, T[] compareStates, T nextState, Func<bool> condition, Action action = null)
        where T : Enum =>
        builder.AddAnyTransition(compareStates.Select(x => x.ToString()).ToArray(), nextState.ToString(), condition, action);

    /// <summary>
    /// Adds a 'state to state'-transition.
    /// The state transition will be executed if the condition is complied.
    /// The action will be executed just once, at the moment when the condition is complied.
    /// </summary>
    /// <param name="builder">The sequence-builder</param>
    /// <param name="description">The transition description (for debugging or just to describe what is it for)</param>
    /// <param name="compareStates">The state(s) that will be compared with the current state.</param>
    /// <param name="nextState">The next state</param>
    /// <param name="condition">The condition</param>
    /// <param name="action">The action that should be executed.</param>
    public static ISequenceBuilder AddAnyTransition<T>(
        this ISequenceBuilder builder, string description, T[] compareStates, T nextState, Func<bool> condition, Action action = null)
        where T : Enum =>
        builder.AddAnyTransition(description, compareStates.Select(x => x.ToString()).ToArray(), nextState.ToString(), condition, action);



    /// <summary>
    /// Adds a state action that should be executed during the state is active.
    /// </summary>
    /// <param name="builder">The sequence-builder</param>
    /// <param name="state">State of the current.</param>
    /// <param name="action">The action.</param>
    /// <param name="condition">The condition that must be fulfilled that the sequence executes the action, default is true.</param>
    public static ISequenceBuilder AddStateAction<T>(this ISequenceBuilder builder, T state, Action action, Func<bool> condition = null)
        where T : Enum =>
        builder.AddStateAction(state.ToString(), action, condition);

    /// <summary>
    /// Adds a state action that should be executed during the state is active.
    /// </summary>
    /// <param name="builder">The sequence-builder</param>
    /// <param name="description">The transition description (for debugging or just to describe what is it for)</param>
    /// <param name="state">State of the current.</param>
    /// <param name="action">The action.</param>
    /// <param name="condition">The condition that must be fulfilled that the sequence executes the action, default is true.</param>
    public static ISequenceBuilder AddStateAction<T>(this ISequenceBuilder builder, string description, T state, Action action, Func<bool> condition = null)
        where T : Enum =>
        builder.AddStateAction(description, state.ToString(), action, condition);



    /// <summary>
    /// Adds a ForceStateHandler to the sequence-handler.
    /// If the condition is fulfilled on execution the CurrentState will be set to the state
    /// and further execution of the sequence will be prevented.
    /// </summary>
    /// <param name="builder">The sequence-builder</param>
    /// <param name="state">The state that should be forced.</param>
    /// <param name="condition">The condition that must be fulfilled that the sequence is forced to the defined state.</param>
    /// <param name="action">The action.</param>
    public static ISequenceBuilder AddForceState<T>(this ISequenceBuilder builder, T state, Func<bool> condition, Action action = null)
        where T : Enum =>
        builder.AddForceState(state.ToString(), condition, action);

    /// <summary>
    /// Adds a ForceStateHandler to the sequence-handler.
    /// If the condition is fulfilled on execution the CurrentState will be set to the state
    /// and further execution of the sequence will be prevented.
    /// </summary>
    /// <param name="builder">The sequence-builder</param>
    /// <param name="description">The transition description (for debugging or just to describe what is it for)</param>
    /// <param name="state">The state that should be forced.</param>
    /// <param name="condition">The condition that must be fulfilled that the sequence is forced to the defined state.</param>
    /// <param name="action">The action.</param>
    public static ISequenceBuilder AddForceState<T>(this ISequenceBuilder builder, string description, T state, Func<bool> condition, Action action = null)
        where T : Enum =>
        builder.AddForceState(description, state.ToString(), condition, action);

     /// <summary>
    /// Adds a StateToggleHandler to the sequence-handler.
    /// If the set condition is fulfilled on execution the CurrentState will be set to the set-state
    /// If the reset condition is fulfilled on execution the CurrentState will be set to the reset-state.
    /// The set is dominant over the reset.
    /// </summary>
    /// <param name="builder">The sequence-builder</param>
    /// <param name="description">The transition description (for debugging or just to describe what is it for)</param>
    /// <param name="fromState">The sequence-state to reset to</param>
    /// <param name="toState">The sequence-state to set to</param>
    /// <param name="dominantSetToCondition">The dominant set-condition that must be fulfilled to execute the state-transition from reset to set-state</param>
    /// <param name="setFromCondition">The reset-condition that must be fulfilled to execute the state-transition from set to reset-state</param>
    /// <param name="setToAction">The action that will be executed after the set-state-transition</param>
    /// <param name="setFromAction">The action that will be executed after the reset-state-transition</param>
    public static ISequenceBuilder AddStateToggle<T>(this ISequenceBuilder builder,
        string description,
        T fromState, T toState,
        Func<bool> dominantSetToCondition, Func<bool> setFromCondition,
        Action setToAction = null, Action setFromAction = null) where T : Enum =>
        builder.AddStateToggle(description, fromState.ToString(), toState.ToString(), dominantSetToCondition, setFromCondition, setToAction, setFromAction);


    /// <summary>
    /// Adds a StateToggleHandler to the sequence-handler.
    /// If the set condition is fulfilled on execution the CurrentState will be set to the set-state
    /// If the reset condition is fulfilled on execution the CurrentState will be set to the reset-state.
    /// The set is dominant over the reset.
    /// </summary>
    /// <param name="builder">The sequence-builder</param>
    /// <param name="fromState">The sequence-state to reset to</param>
    /// <param name="toState">The sequence-state to set to</param>
    /// <param name="dominantSetToCondition">The dominant set-condition that must be fulfilled to execute the state-transition from reset to set-state</param>
    /// <param name="setFromCondition">The reset-condition that must be fulfilled to execute the state-transition from set to reset-state</param>
    /// <param name="setToAction">The action that will be executed after the set-state-transition</param>
    /// <param name="setFromAction">The action that will be executed after the reset-state-transition</param>
    public static ISequenceBuilder AddStateToggle<T>(this ISequenceBuilder builder,
        T fromState, T toState,
        Func<bool> dominantSetToCondition, Func<bool> setFromCondition,
        Action setToAction = null, Action setFromAction = null) where T : Enum =>
        builder.AddStateToggle(builder.DefaultDescription, fromState, toState, dominantSetToCondition, setFromCondition, setToAction, setFromAction);
}
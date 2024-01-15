// ReSharper disable once CheckNamespace, should be reachable from root-namespace (no additional usings are needed)
namespace IegTools.Sequencer;

using System.Linq;
using Handler;

/// <summary>
/// Default String-Extensions für the SequenceBuilder
/// </summary>
public static class SequenceBuilderAddHandlerStringStateExtensions
{
    /// <summary>
    /// Adds a 'state to state'-transition.
    /// The state transition will be executed if the condition is complied.
    /// The action will be executed just once, at the moment when the condition is complied.
    /// </summary>
    /// <param name="builder">The sequence-builder</param>
    /// <param name="currentState">The current state.</param>
    /// <param name="nextState">The next state.</param>
    /// <param name="condition">The condition.</param>
    /// <param name="action">The action that should be executed.</param>
    public static ISequenceBuilder AddTransition(this ISequenceBuilder builder, string currentState, string nextState, Func<bool> condition, Action action = null) =>
        builder.AddTransition(builder.DefaultDescription, currentState, nextState, condition, action);

    /// <summary>
    /// Adds a 'state to state'-transition.
    /// The state transition will be executed if the condition is complied.
    /// The action will be executed just once, at the moment when the condition is complied.
    /// </summary>
    /// <param name="builder">The sequence-builder</param>
    /// <param name="description">The transition description (for debugging or just to describe what is it for)</param>
    /// <param name="currentState">The current state.</param>
    /// <param name="nextState">The next state.</param>
    /// <param name="condition">The condition.</param>
    /// <param name="action">The action that should be executed.</param>
    public static ISequenceBuilder AddTransition(this ISequenceBuilder builder, string description, string currentState, string nextState, Func<bool> condition, Action action = null)
    {
        builder.SetInitialStatesIfTagged(currentState, nextState);
        return builder.AddHandler(
            new StateTransitionHandler(currentState, nextState, condition, action, description));
    }



    /// <summary>
    /// Adds a 'state_s_ to state'-transition.
    /// The state transition will be executed if
    /// the CurrentState contains a substring of the currentStateContains
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
    public static ISequenceBuilder AddContainsTransition(this ISequenceBuilder builder, string currentStateContains, string nextState, Func<bool> condition, Action action = null) =>
        builder.AddContainsTransition(builder.DefaultDescription, currentStateContains, nextState, condition, action);

    /// <summary>
    /// Adds a 'state_s_ to state'-transition.
    /// The state transition will be executed if
    /// the CurrentState contains a substring of the currentStateContains
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
    public static ISequenceBuilder AddContainsTransition(this ISequenceBuilder builder, string description, string currentStateContains, string nextState, Func<bool> condition, Action action = null)
    {
        builder.SetInitialStatesIfTagged(nextState);
        return builder.AddHandler(
            new ContainsStateTransitionHandler(currentStateContains, nextState, condition, action, description));
    }



    /// <summary>
    /// Adds a 'state_s_ to state'-transition.
    /// The state transition will be executed if
    /// the CurrentState is one of the specified compareStates
    /// (eg. CurrentState is compareState1 or compareState2 or...) 
    /// and the condition is complied.
    /// The action will be executed just once, at the moment when the condition is complied.
    /// </summary>
    /// <param name="builder">The sequence-builder</param>
    /// <param name="compareStates">The state(s) that will be compared with the current state.</param>
    /// <param name="nextState">The next state.</param>
    /// <param name="condition">The condition.</param>
    /// <param name="action">The action that should be executed.</param>
    public static ISequenceBuilder AddAnyTransition(this ISequenceBuilder builder, string[] compareStates, string nextState, Func<bool> condition, Action action = null) =>
        builder.AddAnyTransition(builder.DefaultDescription, compareStates, nextState, condition, action);

    /// <summary>
    /// Adds a 'state_s_ to state'-transition.
    /// The state transition will be executed if
    /// the CurrentState is one of the specified compareStates
    /// (eg. CurrentState is compareState1 or compareState2 or...)
    /// and the condition is complied.
    /// The action will be executed just once, at the moment when the condition is complied.
    /// </summary>
    /// <param name="builder">The sequence-builder</param>
    /// <param name="description">The transition description (for debugging or just to describe what is it for)</param>
    /// <param name="compareStates">The state(s) that will be compared with the current state.</param>
    /// <param name="nextState">The next state.</param>
    /// <param name="condition">The condition.</param>
    /// <param name="action">The action that should be executed.</param>
    public static ISequenceBuilder AddAnyTransition(this ISequenceBuilder builder, string description, string[] compareStates, string nextState, Func<bool> condition, Action action = null)
    {
        builder.SetInitialStatesIfTagged(compareStates);
        builder.SetInitialStatesIfTagged(nextState);

        return builder.AddHandler(
            new AnyStateTransitionHandler(compareStates, nextState, condition, action, description));
    }



    /// <summary>
    /// Adds a state action that should be executed during the state is active.
    /// </summary>
    /// <param name="builder">The sequence-builder</param>
    /// <param name="state">The state where the action should be invoked</param>
    /// <param name="action">The action.</param>
    /// <param name="condition">The condition that must be fulfilled that the sequence executes the action, default is true.</param>
    public static ISequenceBuilder AddStateAction(this ISequenceBuilder builder, string state, Action action, Func<bool> condition = null) =>
        builder.AddStateAction(builder.DefaultDescription, state, action, condition);

    /// <summary>
    /// Adds a state action that should be executed during the state is active.
    /// </summary>
    /// <param name="builder">The sequence-builder</param>
    /// <param name="description">The transition description (for debugging or just to describe what is it for)</param>
    /// <param name="state">The state where the action should be invoked</param>
    /// <param name="action">The action.</param>
    /// <param name="condition">The condition that must be fulfilled that the sequence executes the action, default is true.</param>
    public static ISequenceBuilder AddStateAction(this ISequenceBuilder builder, string description, string state, Action action, Func<bool> condition = null)
    {
        builder.SetInitialStatesIfTagged(state);
        return builder.AddHandler(new StateActionHandler(state, condition, action, description));
    }



    /// <summary>
    /// Adds a ForceStateHandler to the sequence-handler.
    /// If the condition is fulfilled on execution the CurrentState will be set to the state
    /// and further execution of the sequence will be prevented.
    /// </summary>
    /// <param name="builder">The sequence-builder</param>
    /// <param name="state">The state that should be forced.</param>
    /// <param name="condition">The condition that must be fulfilled that the sequence is forced to the defined state.</param>
    /// <param name="action">The action.</param>
    public static ISequenceBuilder AddForceState(this ISequenceBuilder builder, string state, Func<bool> condition, Action action = null) =>
        builder.AddForceState(builder.DefaultDescription, state, condition, action);

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
    public static ISequenceBuilder AddForceState(this ISequenceBuilder builder, string description, string state, Func<bool> condition, Action action = null)
    {
        builder.SetInitialStatesIfTagged(state);
        return builder.AddHandler(new ForceStateHandler(state, condition, action, description));
    }

    /// <summary>
    /// Adds a ToggleStatesHandler to the sequence-handler.
    /// If the set condition is fulfilled on execution the CurrentState will be set to the set-state
    /// If the reset condition is fulfilled on execution the CurrentState will be set to the reset-state.
    /// The set is dominant over the reset.
    /// </summary>
    /// <param name="builder">The sequence-builder</param>
    /// <param name="description">The transition description (for debugging or just to describe what is it for)</param>
    /// <param name="resetState">The sequence-state to reset to</param>
    /// <param name="setState">The sequence-state to set to</param>
    /// <param name="dominantSetCondition">The dominant set-condition that must be fulfilled to execute the state-transition from reset to set-state</param>
    /// <param name="resetCondition">The reset-condition that must be fulfilled to execute the state-transition from set to reset-state</param>
    /// <param name="setAction">The action that will be executed after the set-state-transition</param>
    /// <param name="resetAction">The action that will be executed after the reset-state-transition</param>
    public static ISequenceBuilder AddToggleStates(this ISequenceBuilder builder,
        string description,
        string resetState, string setState,
        Func<bool> dominantSetCondition, Func<bool> resetCondition,
        Action setAction = null, Action resetAction = null)
    {
        builder.SetInitialStatesIfTagged(resetState, setState);
        return builder.AddHandler(new ToggleStatesHandler(resetState, setState, dominantSetCondition, resetCondition, setAction, resetAction, description));
    }

    /// <summary>
    /// Adds a ToggleStatesHandler to the sequence-handler.
    /// If the set condition is fulfilled on execution the CurrentState will be set to the set-state
    /// If the reset condition is fulfilled on execution the CurrentState will be set to the reset-state.
    /// The set is dominant over the reset.
    /// </summary>
    /// <param name="builder">The sequence-builder</param>
    /// <param name="resetState">The sequence-state to reset to</param>
    /// <param name="setState">The sequence-state to set to</param>
    /// <param name="dominantSetCondition">The dominant set-condition that must be fulfilled to execute the state-transition from reset to set-state</param>
    /// <param name="resetCondition">The reset-condition that must be fulfilled to execute the state-transition from set to reset-state</param>
    /// <param name="setAction">The action that will be executed after the set-state-transition</param>
    /// <param name="resetAction">The action that will be executed after the reset-state-transition</param>
    public static ISequenceBuilder AddToggleStates(this ISequenceBuilder builder,
        string resetState, string setState,
        Func<bool> dominantSetCondition, Func<bool> resetCondition,
        Action setAction = null, Action resetAction = null) =>
        builder.AddToggleStates(builder.DefaultDescription, resetState, setState, dominantSetCondition, resetCondition, setAction, resetAction);


    private static void SetInitialStatesIfTagged(this ISequenceBuilder builder, params string[] states)
    {
        foreach (var state in states.Where(state => state.StartsWith(builder.InitialStateTag())))
            builder.SetInitialState(state);
    }

    private static string InitialStateTag(this ISequenceBuilder builder) =>
        builder.Configuration.InitialStateTag.ToString();
}
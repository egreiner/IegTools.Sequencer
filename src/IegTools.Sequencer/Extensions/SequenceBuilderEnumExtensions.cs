// ReSharper disable once CheckNamespace, should be reachable from root-namespace (no additional usings are needed)
namespace IegTools.Sequencer;

using System.Linq;
using Rules;

/// <summary>
/// Default Enum-Extensions für the SequenceBuilder
/// </summary>
public static class SequenceBuilderEnumExtensions
{
    /// <summary>
    /// Sets the initial state
    /// </summary>
    /// <param name="builder">The sequence-builder</param>
    /// <param name="initialState">The initial state</param>
    /// <returns></returns>
    public static ISequenceBuilder SetInitialState<T>(this ISequenceBuilder builder, T initialState)
        where T : Enum =>
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
        where T : Enum =>
        builder.AddRule(new StateTransitionRule(currentState.ToString(), nextState.ToString(), constraint, action));

    /// <summary>
    /// Adds a 'state_s_ to state'-transition.
    /// The state transition will be executed if
    /// the CurrentState-string contains a substring of the currentStateContains
    /// (eg1. CurrentState 'ActivatedByApi' contains 'Activated')
    /// (eg2. CurrentState 'ActivatedByApi' contains 'ByApi')
    /// (eg3. CurrentState 'ActivatedByApi' contains 'tedBy')
    /// and the constraint is complied.
    /// The action will be executed just once, at the moment when the constraint is complied.
    /// </summary>
    /// <param name="builder">The sequence-builder</param>
    /// <param name="currentStateContains">Does current-state contains this substring?</param>
    /// <param name="nextState">The next state.</param>
    /// <param name="constraint">The constraint.</param>
    /// <param name="action">The action that should be executed.</param>
    public static ISequenceBuilder AddContainsTransition<T>(
        this ISequenceBuilder builder, string currentStateContains, T nextState, Func<bool> constraint, Action action = null)
        where T : Enum =>
        builder.AddRule(new ContainsStateTransitionRule(currentStateContains, nextState.ToString(), constraint, action));

    /// <summary>
    /// Adds a 'state to state'-transition.
    /// The state transition will be executed if the constraint is complied.
    /// The action will be executed just once, at the moment when the constraint is complied.
    /// </summary>
    /// <param name="builder">The sequence-builder</param>
    /// <param name="compareStates">The state(s) that will be compared with the current state.</param>
    /// <param name="nextState">The next state</param>
    /// <param name="constraint">The constraint</param>
    /// <param name="action">The action that should be executed.</param>
    public static ISequenceBuilder AddAnyTransition<T>(
        this ISequenceBuilder builder, T[] compareStates, T nextState, Func<bool> constraint, Action action = null)
        where T : Enum
    {
        return builder.AddRule(new AnyStateTransitionRule(compareStates.Select(x => x.ToString()).ToArray(), nextState.ToString(), constraint, action));
    }

    /// <summary>
    /// Adds a state action that should be executed during the state is active.
    /// Internal it's handled like a StateTransition...
    /// </summary>
    /// <param name="builder">The sequence-builder</param>
    /// <param name="currentState">State of the current.</param>
    /// <param name="action">The action.</param>
    public static ISequenceBuilder AddStateAction<T>(this ISequenceBuilder builder, T currentState, Action action)
        where T : Enum =>
        builder.AddTransition(currentState, currentState, () => true, action);

    /// <summary>
    /// Adds a ForceStateRule to the sequence-rules.
    /// If the constraint is fulfilled on execution the CurrentState will be set to the state
    /// and further execution of the sequence will be prevented.
    /// </summary>
    /// <param name="builder">The sequence-builder</param>
    /// <param name="state">The state that should be forced.</param>
    /// <param name="constraint">The constraint that must be fulfilled that the sequence is forced to the defined state.</param>
    /// <param name="action">The action.</param>
    public static ISequenceBuilder AddForceState<T>(this ISequenceBuilder builder, T state, Func<bool> constraint, Action action = null)
        where T : Enum =>
        builder.AddRule(new ForceStateRule(state.ToString(), constraint, action));


    /// <summary>
    /// Does not validate statuses that are in this list
    /// </summary>
    /// <param name="builder">The sequence-builder</param>
    /// <param name="statuses">A list of statuses that should not be validated.</param>
    public static ISequenceBuilder DisableValidationForStates<T>(this ISequenceBuilder builder, params T[] statuses)
        where T : Enum
    {
        builder.Configuration.DisableValidationForStatuses = statuses.Select(x1 => x1.ToString()).ToArray();
        return builder;
    }
}
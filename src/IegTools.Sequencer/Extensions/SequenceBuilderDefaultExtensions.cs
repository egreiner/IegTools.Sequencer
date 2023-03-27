﻿// ReSharper disable once CheckNamespace, should be reachable from root-namespace (no additional usings are needed)
namespace IegTools.Sequencer;

using System.Linq;
using Rules;

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
        return builder.AddRule(
            new StateTransitionRule(currentState, nextState, constraint, action));
    }

    /// <summary>
    /// Adds a 'state_s_ to state'-transition.
    /// The state transition will be executed if
    /// the CurrentState contains a substring of the currentStateContains
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
    public static ISequenceBuilder AddContainsTransition(this ISequenceBuilder builder, string currentStateContains, string nextState, Func<bool> constraint, Action action = null)
    {
        builder.AddInitialStates(nextState);
        return builder.AddRule(
            new ContainsStateTransitionRule(currentStateContains, nextState, constraint, action));
    }

    /// <summary>
    /// Adds a 'state_s_ to state'-transition.
    /// The state transition will be executed if
    /// the CurrentState is one of the specified compareStates
    /// (eg. CurrentState is compareState1 or compareState2 or...) 
    /// and the constraint is complied.
    /// The action will be executed just once, at the moment when the constraint is complied.
    /// </summary>
    /// <param name="builder">The sequence-builder</param>
    /// <param name="compareStates">The state(s) that will be compared with the current state.</param>
    /// <param name="nextState">The next state.</param>
    /// <param name="constraint">The constraint.</param>
    /// <param name="action">The action that should be executed.</param>
    public static ISequenceBuilder AddAnyTransition(this ISequenceBuilder builder, string[] compareStates, string nextState, Func<bool> constraint, Action action = null)
    {
        builder.AddInitialStates(compareStates);
        builder.AddInitialStates(nextState);
        
        return builder.AddRule(
            new AnyStateTransitionRule(compareStates, nextState, constraint, action));


        ////foreach (var state in compareStates)
        ////    builder.AddTransition(state, nextState, constraint, action);

        ////return builder;
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
        return builder.AddRule(new StateActionRule(state, action));
    }

    /// <summary>
    /// Adds a ForceStateRule to the sequence-rules.
    /// If the constraint is fulfilled on execution the CurrentState will be set to the state
    /// and further execution of the sequence will be prevented.
    /// </summary>
    /// <param name="builder">The sequence-builder</param>
    /// <param name="state">The state that should be forced.</param>
    /// <param name="constraint">The constraint that must be fulfilled that the sequence is forced to the defined state.</param>
    public static ISequenceBuilder AddForceState(this ISequenceBuilder builder, string state, Func<bool> constraint)
    {
        builder.AddInitialStates(state);
        return builder.AddRule(new ForceStateRule(state, constraint));
    }


    private static void AddInitialStates(this ISequenceBuilder builder, params string[] statuses)
    {
        foreach (var state in statuses.Where(state => state.StartsWith(builder.InitialStateTag())))
            builder.SetInitialState(state);
    }

    private static string InitialStateTag(this ISequenceBuilder builder) =>
        builder.Configuration.InitialStateTag.ToString();
}
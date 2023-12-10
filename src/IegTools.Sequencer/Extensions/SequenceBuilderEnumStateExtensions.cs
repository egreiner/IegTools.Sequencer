// ReSharper disable once CheckNamespace, should be reachable from root-namespace (no additional usings are needed)
namespace IegTools.Sequencer;

using System.Linq;

/// <summary>
/// Default Enum-Extensions für the SequenceBuilder.
/// Forward all calls to <see cref="SequenceBuilderAddHandlerStringStateExtensions"/>
/// </summary>
public static class SequenceBuilderEnumStateExtensions
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
    /// Does not validate states that are in this list
    /// </summary>
    /// <param name="builder">The sequence-builder</param>
    /// <param name="states">A list of states that should not be validated.</param>
    public static ISequenceBuilder DisableValidationForStates<T>(this ISequenceBuilder builder, params T[] states)
        where T : Enum
    {
        builder.Configuration.DisableValidationForStates = states.Select(x1 => x1.ToString()).ToArray();
        return builder;
    }
}
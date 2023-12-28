// ReSharper disable once CheckNamespace, should be reachable from root-namespace (no additional usings are needed)
namespace IegTools.Sequencer;

using System.Linq;

/// <summary>
/// The AllowOnlyOnceIn-Extension
/// </summary>
public static class AllowOnlyOnceInExtension
{
    /// <summary>
    /// Adds a constraint to the last added handler that it should be executed only once in the defined timespan
    /// </summary>
    /// <param name="builder">The sequence-builder</param>
    /// <param name="timeSpan">The timespan in which the execution of the transition action is allowed only once</param>
    public static ISequenceBuilder AllowOnlyOnceIn(this ISequenceBuilder builder, TimeSpan timeSpan)
    {
        builder.Data.Handler.LastOrDefault()?.AllowOnlyOnceIn(timeSpan);
        return builder;
    }
}
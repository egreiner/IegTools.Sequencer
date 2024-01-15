// ReSharper disable once CheckNamespace, should be reachable from root-namespace (no additional usings are needed)
namespace IegTools.Sequencer;

using System.Linq;

/// <summary>
/// Some string ExtensionMethods
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Returns true if the string is in the given string-values
    /// </summary>
    /// <param name="text">The text</param>
    /// <param name="stringValues">The string-values that should be compared</param>
    public static bool IsIn(this string text, params string[] stringValues) =>
        stringValues.Contains(text);

    /// <summary>
    /// Returns true if the string is in the given string-array or string-values
    /// </summary>
    /// <param name="text">The text</param>
    /// <param name="stringArray">The string-array that should be compared</param>
    /// <param name="stringValues">The string-values that should be compared</param>
    public static bool IsIn(this string text, string[] stringArray, params string[] stringValues) =>
        stringArray.Contains(text) || stringValues.Contains(text);
}
// ReSharper disable once CheckNamespace, should be reachable from root-namespace (no additional usings are needed)
namespace IegTools.Sequencer;

using FluentValidation.Results;

/// <summary>
/// Extensions for the validation result.
/// </summary>
public static class ValidationResultExtensions
{
    /// <summary>
    /// Adds an error to the validation result.
    /// </summary>
    /// <param name="result">The validation result</param>
    /// <param name="propertyName">The name of the erroneous property</param>
    /// <param name="message">The validation error message</param>
    public static void AddError(this ValidationResult result, string propertyName, string message) =>
        result.Errors.Add(new ValidationFailure(propertyName, message));
}
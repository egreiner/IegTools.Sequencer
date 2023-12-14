namespace IegTools.Sequencer.Validation;

using FluentValidation;
using FluentValidation.Results;

/// <summary>
/// The handler validator base interface.
/// </summary>
public interface IHandlerValidator
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="result"></param>
    /// <returns>true if valid</returns>
    bool Validate(ValidationContext<SequenceBuilder> builder, ValidationResult result);
}
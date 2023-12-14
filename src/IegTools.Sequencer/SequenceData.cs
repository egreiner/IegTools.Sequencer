namespace IegTools.Sequencer;

using System.Collections.Generic;
using Handler;
using Validation;

/// <summary>
/// The sequence data
/// </summary>
public class SequenceData
{
    /// <summary>
    /// The sequence-handler that describe how the sequence is supposed to work
    /// </summary>
    public List<IHandler> Handler { get; } = new();

    /// <summary>
    /// All validators
    /// </summary>
    public IList<IHandlerValidator> Validators { get; set; } = new List<IHandlerValidator>();
}
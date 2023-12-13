namespace IegTools.Sequencer;

using Microsoft.Extensions.Logging;
using Handler;
using JetBrains.Annotations;
using Validation;

#nullable enable

/// <summary>
/// The Sequence-Builder interface
/// </summary>
public interface ISequenceBuilder
{
    /// <summary>
    /// The sequence configuration
    /// </summary>
    SequenceConfiguration Configuration { get; init; }


    /// <summary>
    /// Activates debug logging messages
    /// </summary>
    /// <param name="logger">The logger</param>
    /// <param name="eventId">The EventId</param>
    /// <param name="loggerScope">The logger-scope</param>
    ISequenceBuilder ActivateDebugLogging(ILogger logger, EventId eventId, Func<IDisposable>? loggerScope = null);

    /// <summary>
    /// Adds an sequence handler
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="handler">The handler.</param>
    ISequenceBuilder AddHandler<T>(T handler) where T: IHandler;

    /// <summary>
    /// Adds an Handler-Validator
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    ISequenceBuilder AddValidator<T>() where T : IHandlerValidator, new();

    /// <summary>
    /// Builds a default sequence with the specified configuration
    /// Throws an exception if the sequence is not valid.
    /// </summary>
    ISequence Build();

    /// <summary>
    /// Builds a customized sequence with the specified configuration
    /// Throws an exception if the sequence is not valid.
    /// </summary>
    ISequence Build<TSequence>() where TSequence : ISequence, new();

    /// <summary>
    /// Does not validate the sequence configuration on build
    /// </summary>
    ISequenceBuilder DisableValidation();

    /// <summary>
    /// Does not validate states that are in this list
    /// </summary>
    /// <param name="states">A list of states that should not be validated.</param>
    ISequenceBuilder DisableValidationForStates(params string[] states);

    /// <summary>
    /// Sets the initial state
    /// </summary>
    /// <param name="initialState">The initial state</param>
    ISequenceBuilder SetInitialState(string initialState);
}
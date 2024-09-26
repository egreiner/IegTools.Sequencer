namespace IegTools.Sequencer.Logging;

#nullable enable
using System;
using Handler;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

/// <summary>
/// The logger adapter interface.
/// </summary>
public interface ILoggerAdapter : ILogger
{
    /// <summary>
    /// The logger
    /// </summary>
    ILogger? Logger { get; }

    /// <summary>
    /// The logger scope
    /// </summary>
    Func<IDisposable>? LoggerScope { get; }

    /// <summary>
    /// The EventId for logging
    /// </summary>
    EventId EventId { get; set; }


    /// <summary>
    /// Log information
    /// </summary>
    /// <param name="eventId"></param>
    /// <param name="message"></param>
    /// <param name="parameter"></param>
    void LogInformation(EventId eventId, [StructuredMessageTemplate] string? message, params object?[] parameter);

    /// <summary>
    /// Log trace
    /// </summary>
    /// <param name="eventId"></param>
    /// <param name="message"></param>
    /// <param name="parameter"></param>
    void LogTrace(EventId eventId, [StructuredMessageTemplate] string? message, params object?[] parameter);

    /// <summary>
    /// Log debug
    /// </summary>
    /// <param name="eventId"></param>
    /// <param name="message"></param>
    /// <param name="parameter"></param>
    void LogDebug(EventId eventId, [StructuredMessageTemplate] string? message, params object?[] parameter);

    /// <summary>
    ///
    /// </summary>
    /// <param name="eventId"></param>
    /// <param name="e"></param>
    /// <param name="message"></param>
    /// <param name="parameter"></param>
    void LogWarning(EventId eventId, Exception e, [StructuredMessageTemplate] string? message, params object?[] parameter);
    
    /// <summary>
    /// Log error
    /// </summary>
    /// <param name="eventId"></param>
    /// <param name="e"></param>
    /// <param name="message"></param>
    /// <param name="parameter"></param>
    void LogError(EventId eventId, Exception e,[StructuredMessageTemplate] string? message, params object?[] parameter);

    /// <summary>
    /// Get the internal logger scope
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="methodName"></param>
    IDisposable? GetSequenceLoggerScope(IHandler handler, string methodName);
}
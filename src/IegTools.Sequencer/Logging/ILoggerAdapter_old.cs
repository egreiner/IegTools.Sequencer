namespace IegTools.Sequencer.Logging;

#nullable enable

using System;
using Handler;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

public interface ILoggerAdapter //: ILogger<TCLass>
// public interface ILoggerAdapter<out TCLass> //: ILogger<TCLass>
{
    // ILogger<TCLass> Logger { get; }

    ILoggerAdapter Set(Func<IDisposable> loggerScope);

    void LogInformation(EventId eventId, [StructuredMessageTemplate] string? message, params object?[] parameter);
    void LogTrace(EventId eventId, [StructuredMessageTemplate] string? message, params object?[] parameter);
    void LogDebug(EventId eventId, [StructuredMessageTemplate] string? message, params object?[] parameter);

    void LogWarning(EventId eventId, Exception e, [StructuredMessageTemplate] string? message, params object?[] parameter);
    
    void LogError(EventId eventId, Exception e,[StructuredMessageTemplate] string? message, params object?[] parameter);

    /// <summary>
    /// Returns a logger scope
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="methodName">The method-name</param>
    IDisposable GetLoggerScope(IHandler handler, string methodName);
}
namespace IegTools.Sequencer.Logging;

#nullable enable

using System;
using System.Collections.Generic;
using Handler;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

/// <summary>
/// The logger-adapter
/// </summary>
/// <typeparam name="TClass"></typeparam>
public class LoggerAdapterOld<TClass> : ILoggerAdapter
{
    private readonly ILogger<TClass>?   _logger;
    private          Func<IDisposable>? _loggerScope;

    /// <summary>
    /// The logger-adapter
    /// </summary>
    public LoggerAdapterOld() { }

    /// <summary>
    /// The logger-adapter
    /// </summary>
    /// <param name="logger"></param>
    /// <typeparam name="TClass"></typeparam>
    public LoggerAdapterOld(ILogger<TClass> logger) =>
        _logger = logger;


    public ILoggerAdapter Set(Func<IDisposable>? loggerScope)
    {
        _loggerScope = loggerScope;
        return this;
    }


    /// <summary>
    /// Returns a logger scope
    /// </summary>
    /// <param name="handler">The handler</param>
    /// <param name="methodName">The method-name</param>
    public IDisposable GetLoggerScope(IHandler handler, string methodName) =>
        _logger?.BeginScope(new Dictionary<string, object>
        {
            { "Description", handler?.Description ?? string.Empty },
            { "SequenceStopwatch", handler?.Sequence.Stopwatch.Elapsed ?? TimeSpan.Zero },
            { "Method", methodName },
        });


    public void LogInformation(EventId eventId, [StructuredMessageTemplate] string? message, params object?[] parameter)
    {
        using var loggerScope = _loggerScope?.Invoke();
        _logger?.LogInformation(eventId, message, parameter);
    }

    public void LogTrace(EventId eventId, [StructuredMessageTemplate] string? message, params object?[] parameter)
    {
        using var loggerScope = _loggerScope?.Invoke();
        _logger?.LogTrace(eventId, message, parameter);
    }

    public void LogDebug(EventId eventId, [StructuredMessageTemplate] string? message, params object?[] parameter)
    {
        using var loggerScope = _loggerScope?.Invoke();
        _logger?.LogDebug(eventId, message, parameter);
    }


    public void LogWarning(EventId eventId, Exception e, [StructuredMessageTemplate] string? message, params object?[] parameter)
    {
        using var loggerScope = _loggerScope?.Invoke();
        _logger?.LogWarning(eventId, e, message, parameter);
    }

    public void LogError(EventId eventId, Exception? e, [StructuredMessageTemplate] string? message, params object?[] parameter)
    {
        using var loggerScope = _loggerScope?.Invoke();
        _logger?.LogError(eventId, e, message, parameter);
    }



    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        using var loggerScope = _loggerScope?.Invoke();
        _logger?.Log(logLevel, eventId, state, exception, formatter);
    }

    public bool IsEnabled(LogLevel logLevel) => _logger?.IsEnabled(logLevel) ?? false;

    public IDisposable BeginScope<TState>(TState state) => _logger?.BeginScope(state);
}
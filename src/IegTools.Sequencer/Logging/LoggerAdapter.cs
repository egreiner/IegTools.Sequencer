namespace IegTools.Sequencer.Logging;

#nullable enable
using System;
using System.Collections.Generic;
using Handler;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

/// <summary>
/// The logger adapter
/// </summary>
public class LoggerAdapter : ILoggerAdapter
{
    /// <summary>
    /// The logger-adapter
    /// </summary>
    public LoggerAdapter() { }

    /// <summary>
    /// The logger-adapter
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="eventId"></param>
    /// <param name="loggerScope"></param>
    public LoggerAdapter(ILogger? logger, EventId eventId, Func<IDisposable>? loggerScope)
    {
        Logger      = logger;
        EventId     = eventId;
        LoggerScope = loggerScope;
    }


    /// <inheritdoc />
    public ILogger? Logger { get; }

    /// <inheritdoc />
    public EventId EventId { get; set; }

    /// <inheritdoc />
    public Func<IDisposable>? LoggerScope { get; private set; }



    /// <inheritdoc />
    public void LogInformation(EventId eventId, [StructuredMessageTemplate] string? message, params object?[] parameter)
    {
        using var externalScope = LoggerScope?.Invoke();
        Logger?.LogInformation(eventId, message, parameter);
    }

    /// <inheritdoc />
    public void LogTrace(EventId eventId, [StructuredMessageTemplate] string? message, params object?[] parameter)
    {
        using var loggerScope = LoggerScope?.Invoke();
        Logger?.LogTrace(eventId, message, parameter);
    }

    /// <inheritdoc />
    public void LogDebug(EventId eventId, [StructuredMessageTemplate] string? message, params object?[] parameter)
    {
        using var loggerScope = LoggerScope?.Invoke();
        Logger?.LogDebug(eventId, message, parameter);
    }


    /// <inheritdoc />
    public void LogWarning(EventId eventId, Exception e, [StructuredMessageTemplate] string? message,
        params object?[]           parameter)
    {
        using var loggerScope = LoggerScope?.Invoke();
        Logger?.LogWarning(eventId, e, message, parameter);
    }

    /// <inheritdoc />
    public void LogError(EventId eventId, Exception? e, [StructuredMessageTemplate] string? message,
        params object?[]         parameter)
    {
        using var loggerScope = LoggerScope?.Invoke();
        Logger?.LogError(eventId, e, message, parameter);
    }


    /// <inheritdoc />
    public void Log<TState>(LogLevel     logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        using var loggerScope = LoggerScope?.Invoke();
        Logger?.Log(logLevel, eventId, state, exception, formatter);
    }

    /// <inheritdoc />
    public bool IsEnabled(LogLevel logLevel) => Logger?.IsEnabled(logLevel) ?? false;

    /// <inheritdoc />
    public IDisposable? BeginScope<TState>(TState state) => Logger?.BeginScope(state) ?? null;



    /// <inheritdoc />
    public IDisposable? GetSequenceLoggerScope(IHandler handler, string methodName) =>
        Logger?.BeginScope(new Dictionary<string, object>
        {
            { "Description", handler?.Description ?? string.Empty },
            { "SequenceStopwatch", handler?.Sequence.Stopwatch.Elapsed ?? TimeSpan.Zero },
            { "Method", methodName },
        }) ?? null;}
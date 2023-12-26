namespace UnitTests.Sequencer.Logging;

using IegTools.Sequencer.Logging;


public class LoggerAdapterTests
{
    [Fact]
    public void Should_be_enabled()
    {
        var logger        = Substitute.For<ILogger<LoggingTests>>();
        var loggerAdapter = new LoggerAdapter(logger, new EventId(-1, "Test EventId"), loggerScope: null);

        var actual = loggerAdapter.IsEnabled(LogLevel.Trace);

        actual.Should().BeFalse();
    }

    [Fact]
    public void Should_send_log_message()
    {
        var logger        = Substitute.For<ILogger<LoggingTests>>();
        var loggerAdapter = new LoggerAdapter(logger, new EventId(-1, "Test EventId"), loggerScope: null);

        loggerAdapter.Log(LogLevel.Information, new EventId(-1, "Test EventId"), "Test message");

        logger.Received(1).Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    [Fact]
    public void Should_send_log_message_Information()
    {
        var logger        = Substitute.For<ILogger<LoggingTests>>();
        var loggerAdapter = new LoggerAdapter(logger, new EventId(-1, "Test EventId"), loggerScope: null);

        loggerAdapter.LogInformation(new EventId(-1, "Test EventId"), "Test message");

        logger.Received(1).Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    [Fact]
    public void Should_send_log_message_Warning()
    {
        var logger        = Substitute.For<ILogger<LoggingTests>>();
        var loggerAdapter = new LoggerAdapter(logger, new EventId(-1, "Test EventId"), loggerScope: null);

        loggerAdapter.LogWarning(new EventId(-1, "Test EventId"), new Exception("Test"), "Test message");

        logger.Received(1).Log(
            LogLevel.Warning,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    [Fact]
    public void Should_send_log_message_Error()
    {
        var logger        = Substitute.For<ILogger<LoggingTests>>();
        var loggerAdapter = new LoggerAdapter(logger, new EventId(-1, "Test EventId"), loggerScope: null);

        loggerAdapter.LogError(new EventId(-1, "Test EventId"), new Exception("Test"), "Test message");

        logger.Received(1).Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    [Fact]
    public void Should_send_log_message_Debug()
    {
        var logger        = Substitute.For<ILogger<LoggingTests>>();
        var loggerAdapter = new LoggerAdapter(logger, new EventId(-1, "Test EventId"), loggerScope: null);

        loggerAdapter.LogDebug(new EventId(-1, "Test EventId"), "Test message");

        logger.Received(1).Log(
            LogLevel.Debug,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    [Fact]
    public void Should_send_log_message_Trace()
    {
        var logger        = Substitute.For<ILogger<LoggingTests>>();
        var loggerAdapter = new LoggerAdapter(logger, new EventId(-1, "Test EventId"), loggerScope: null);

        loggerAdapter.LogTrace(new EventId(-1, "Test EventId"), "Test message");

        logger.Received(1).Log(
            LogLevel.Trace,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }


    [Fact]
    public void Should_send_logger_scope()
    {
        var                logger = Substitute.For<ILogger<LoggingTests>>();
        Func<IDisposable>? scope() => Substitute.For<Func<IDisposable>>();
        var                loggerAdapter = new LoggerAdapter(logger, new EventId(-1, "Test EventId"), loggerScope: scope());

        loggerAdapter.LogInformation(new EventId(-1, "Test EventId"), "Test message");

        logger.Received(1).Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }
}
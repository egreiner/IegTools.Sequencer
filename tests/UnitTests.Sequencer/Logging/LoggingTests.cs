namespace UnitTests.Sequencer.Logging;

using NSubstitute.Extensions;

public class LoggingTests
{
    [Fact]
    public void Should_not_send_any_log_messages()
    {
        var logger = Substitute.For<ILogger<LoggingTests>>();

        _ = SequenceBuilder.Configure(config =>
            config.SetInitialState("Off")
                .SetLogger(logger, new EventId(-1))
        );

        logger.Received(0).Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }


    [Fact]
    public void Should_not_send_any_log_messages_LogLevel_Information()
    {
        var logger = Substitute.For<ILogger<LoggingTests>>();

        _ = SequenceBuilder.Configure(config =>
            config.SetInitialState("Off")
                .SetLogger(logger, new EventId(-1, "Seq 1"))
        );

        logger.Received(0).Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    [Fact]
    public void Should_not_send_any_log_messages_LogLevel_Debug()
    {
        var logger = Substitute.For<ILogger<LoggingTests>>();

        _ = SequenceBuilder.Configure(config =>
            config.SetInitialState("Off")
                .SetLogger(logger, new EventId(-1, "Seq 1"), LogLevel.Debug)
        );

        logger.Received(0).Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    [Fact]
    public void Should_send_log_messages_force_state()
    {
        // Test
        Assert.False(true);

        // using ILoggerFactory factory = LoggerFactory.Create(builder => builder.ClearProviders());

        var logger  = Substitute.For<ILogger<LoggingTests>>();

        var builder = SequenceBuilder.Configure(config =>
            {
                config.SetInitialState("Off");
                config.SetLogger(logger, new EventId(-1, "Seq 1"), LogLevel.Warning);
                config.DisableValidation();

                config.AddForceState("Test", () => true);
            }
        );

        var sequence = builder.Build();
        sequence.Run();

        logger.Received(1).Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

}
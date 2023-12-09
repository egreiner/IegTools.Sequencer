namespace UnitTests.Sequencer.Logging;

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

    // [Fact(Skip="Obsolete")]
    // public void Should_send_the_expected_log_message()
    // {
    //     var logger = Substitute.For<ILogger<LoggingTests>>();
    //     _ = SequenceBuilder.Configure(config =>
    //         config.SetInitialState("Off")
    //             .SetLogger(logger, "Service is enabled")
    //     );
    //
    //     // Assert
    //     logger.Received(1).Log(
    //         LogLevel.Information,
    //         Arg.Any<EventId>(),
    //         Arg.Is<object>(o => o.ToString() == "Service is enabled"),
    //         Arg.Any<Exception>(),
    //         Arg.Any<Func<object, Exception?, string>>());
    // }
    //
    // [Fact(Skip="Obsolete")]
    // public void Should_send_the_expected_log_message_with_args()
    // {
    //     var logger = Substitute.For<ILogger<LoggingTests>>();
    //     _ = SequenceBuilder.Configure(config =>
    //         config.SetInitialState("Off")
    //             .SetLogger(logger, "Service is {Status}", "disabled")
    //     );
    //
    //     // Assert
    //     logger.Received(1).Log(
    //         LogLevel.Information,
    //         Arg.Any<EventId>(),
    //         Arg.Is<object>(o => o.ToString() == "Service is disabled"),
    //         Arg.Any<Exception>(),
    //         Arg.Any<Func<object, Exception?, string>>());
    // }
}
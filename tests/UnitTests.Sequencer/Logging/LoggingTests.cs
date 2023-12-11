namespace UnitTests.Sequencer.Logging;

using Microsoft.Extensions.DependencyInjection;
using NSubstitute.Extensions;
using Xunit.Abstractions;

public class LoggingTests
{
    private ITestOutputHelper _output;

    public LoggingTests(ITestOutputHelper output) =>
        _output = output;

    [Fact]
    public void Should_not_send_any_log_messages()
    {
        var logger = Substitute.For<ILogger<LoggingTests>>();

        _ = SequenceBuilder.Configure(config =>
            config.SetInitialState("Off")
                .ActivateDebugLogging(logger, new EventId(-1, "Test EventId"))
        );

        logger.Received(0).Log(
            LogLevel.Debug,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    [Fact(Skip="wip")]
    public void Should_send_log_messages_with_Description()
    {
        // var serviceProvider = new ServiceCollection()
        //     .AddLogging(builder =>
        //     {
        //         builder.SetMinimumLevel(LogLevel.Debug);
        //     })
        //     .BuildServiceProvider();

        var logger = Substitute.For<ILogger<LoggingTests>>();

        var builder = SequenceBuilder.Configure(config =>
            config.SetInitialState(TestEnum.InitialState)
                  .ActivateDebugLogging(logger, new EventId(-1, "Test EventId"))
                  .DisableValidation()
                  .AddTransition("Test description", TestEnum.State1, TestEnum.State2, () => true)
        );

        var sequence = builder.Build();
        sequence.Run();

        logger.Received(1).Log(
            LogLevel.Debug,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }


    [Fact(Skip="wip")]
    public void Should_send_log_messages_force_state()
    {
        // Test
        // Assert.False(true);

        // using ILoggerFactory factory = LoggerFactory.Create(builder => builder.ClearProviders());

        var logger  = Substitute.For<ILogger<LoggingTests>>();

        var builder = SequenceBuilder.Configure(config =>
            {
                config.SetInitialState("Off");
                config.ActivateDebugLogging(logger, new EventId(-1, "Seq 1"));
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
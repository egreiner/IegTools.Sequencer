namespace UnitTests.Sequencer.Logging;

public class LoggingTests
{
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

    [Fact]
    public void Should_send_log_messages_for_Transition_with_Description()
    {
        var logger = Substitute.For<ILogger<LoggingTests>>();
        var sequence = SequenceBuilder.Configure(config =>
            config.SetInitialState(TestEnum.InitialState)
                .ActivateDebugLogging(logger, new EventId(-1, "Test EventId"))
                .DisableValidation()
                .AddTransition("Test description", TestEnum.InitialState, TestEnum.State2, () => true)
        ).Build();

        sequence.Run();

        logger.Received(1).Log(
            LogLevel.Debug,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    [Fact]
    public void Should_send_log_messages_for_ForceState_with_Description()
    {
        var logger = Substitute.For<ILogger<LoggingTests>>();
        var sequence = SequenceBuilder.Configure(config =>
            config.SetInitialState(TestEnum.InitialState)
                .ActivateDebugLogging(logger, new EventId(-1, "Test EventId"))
                .DisableValidation()
                .AddForceState("Test description", TestEnum.State2, () => true)
        ).Build();

        sequence.Run();

        logger.Received(1).Log(
            LogLevel.Debug,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    [Fact]
    public void Should_send_log_messages_for_ContainsTransition_with_Description()
    {
        var logger = Substitute.For<ILogger<LoggingTests>>();
        var sequence = SequenceBuilder.Configure(config =>
            config.SetInitialState(TestEnum.InitialState)
                .ActivateDebugLogging(logger, new EventId(-1, "Test EventId"))
                .DisableValidation()
                .AddContainsTransition("Test description", "Init", TestEnum.State2, () => true)
        ).Build();

        sequence.Run();

        logger.Received(1).Log(
            LogLevel.Debug,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    [Fact]
    public void Should_send_log_messages_for_AnyTransition_with_Description()
    {
        var logger = Substitute.For<ILogger<LoggingTests>>();
        var sequence = SequenceBuilder.Configure(config =>
            config.SetInitialState(TestEnum.InitialState)
                .ActivateDebugLogging(logger, new EventId(-1, "Test EventId"))
                .DisableValidation()
                .AddAnyTransition("Test description", [TestEnum.InitialState,TestEnum.State1], TestEnum.State2, () => true)
        ).Build();

        sequence.Run();

        logger.Received(1).Log(
            LogLevel.Debug,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    [Fact]
    public void Should_send_log_messages_for_StateAction_with_Description()
    {
        var x      = 0;
        var logger = Substitute.For<ILogger<LoggingTests>>();
        var sequence = SequenceBuilder.Configure(config =>
            config.SetInitialState(TestEnum.InitialState)
                .ActivateDebugLogging(logger, new EventId(-1, "Test EventId"))
                .DisableValidation()
                .AddStateAction("Test description", TestEnum.InitialState, () => x = 100, () => true)
        ).Build();

        sequence.Run();

        x.Should().Be(100);
        logger.Received(1).Log(
            LogLevel.Debug,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }
}
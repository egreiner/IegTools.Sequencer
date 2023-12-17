namespace UnitTests.Sequencer.Validation.HandlerValidators;

public class DebugLoggingValidatorTests
{
    [Fact]
    public void Test_DoesNotThrowValidationError()
    {
        var logger = Substitute.For<ILogger<DebugLoggingValidatorTests>>();
        var builder = SequenceBuilder.Configure(builder =>
            {
                builder.SetInitialState("Off");
                builder.ActivateDebugLogging(logger, new EventId(-1, "Seq 1"));
                builder.DisableValidation();

                builder.AddForceState("Test", () => true);
            }
        );

        FluentActions.Invoking(() => builder.Build())
            .Should().NotThrow<FluentValidation.ValidationException>();
    }

    [Fact]
    public void Test_DoesThrowValidationError_NoDescription()
    {
        var logger = Substitute.For<ILogger<DebugLoggingValidatorTests>>();
        var builder = SequenceBuilder.Configure(builder =>
            {
                builder.SetInitialState("Off");
                builder.ActivateDebugLogging(logger, new EventId(-1, "Seq 1"));

                builder.AddForceState("Test", () => true);
            }
        );

        FluentActions.Invoking(() => builder.Build())
            .Should().Throw<FluentValidation.ValidationException>()
            .WithMessage("*sequence must have a meaningful description*");
    }


    [Fact]
    public void Test_DoesThrowValidationError_EmptyDescription()
    {
        var logger = Substitute.For<ILogger<DebugLoggingValidatorTests>>();
        var builder = SequenceBuilder.Configure(builder =>
            {
                builder.SetInitialState("Off");
                builder.ActivateDebugLogging(logger, new EventId(-1, "Seq 1"));

                builder.AddForceState("", "Test", () => true);
            }
        );

        FluentActions.Invoking(() => builder.Build())
            .Should().Throw<FluentValidation.ValidationException>()
            .WithMessage("*sequence must have a meaningful description*");
    }

    [Fact]
    public void Test_DoesThrowValidationError_NullDescription()
    {
        var logger = Substitute.For<ILogger<DebugLoggingValidatorTests>>();
        var builder = SequenceBuilder.Configure(builder =>
            {
                builder.SetInitialState("Off");
                builder.ActivateDebugLogging(logger, new EventId(-1, "Seq 1"));

                builder.AddForceState(null, "Test", () => true);
            }
        );

        FluentActions.Invoking(() => builder.Build())
            .Should().Throw<FluentValidation.ValidationException>()
            .WithMessage("*sequence must have a meaningful description*");
    }
}
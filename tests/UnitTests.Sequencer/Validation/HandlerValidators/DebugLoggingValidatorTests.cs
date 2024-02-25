namespace UnitTests.Sequencer.Validation.HandlerValidators;

using IegTools.Sequencer.Validation;

public class DebugLoggingValidatorTests
{
    [Fact]
    public void Should_not_throw_ValidationError_Add_Description()
    {
        var logger = Substitute.For<ILogger<DebugLoggingValidatorTests>>();
        var builder = SequenceBuilder.Configure(builder =>
            {
                builder.WithValidator<DebugLoggingValidator>();

                builder.SetInitialState("Off");
                builder.DisableValidationForStates("Off", "Test1", "Test2");
                builder.ActivateDebugLogging(logger, new EventId(-1, "Seq 1"));

                builder.AddForceState("Description for the force state Test 1", "Test1", () => true);
                builder.AddForceState("Description for the force state Test 2", "Test2", () => true);
            }
        );

        FluentActions.Invoking(() => builder.Build())
            .Should().NotThrow<FluentValidation.ValidationException>();
    }

    [Fact]
    public void Should_not_throw_ValidationError_DisableValidation()
    {
        var logger = Substitute.For<ILogger<DebugLoggingValidatorTests>>();
        var builder = SequenceBuilder.Configure(builder =>
            {
                builder.WithValidator<DebugLoggingValidator>();

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
    public void Should_throw_ValidationError_NoDescription()
    {
        var logger = Substitute.For<ILogger<DebugLoggingValidatorTests>>();
        var builder = SequenceBuilder.Configure(builder =>
            {
                builder.WithValidator<DebugLoggingValidator>();

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
    public void Should_throw_ValidationError_EmptyDescription()
    {
        var logger = Substitute.For<ILogger<DebugLoggingValidatorTests>>();
        var builder = SequenceBuilder.Configure(builder =>
            {
                builder.WithValidator<DebugLoggingValidator>();

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
    public void Should_throw_ValidationError_NullDescription()
    {
        var logger = Substitute.For<ILogger<DebugLoggingValidatorTests>>();
        var builder = SequenceBuilder.Configure(builder =>
            {
                builder.WithValidator<DebugLoggingValidator>();

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
﻿namespace UnitTests.Sequencer.Validation.HandlerValidators;

using IegTools.Sequencer.Validation;

public class DebugLoggingValidatorTests
{
    [Fact]
    public void Build_ShouldNotThrowValidationError_When_DescriptionIsValid()
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

        var build = () => builder.Build();

        build.Should().NotThrow<FluentValidation.ValidationException>();
    }

    [Fact]
    public void Build_ShouldNotThrowValidationError_When_DisableValidation()
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

        var build = () => builder.Build();

        build.Should().NotThrow<FluentValidation.ValidationException>();
    }

    [Fact]
    public void Build_ShouldThrowValidationError_When_DescriptionIsMissing()
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

        var build = () => builder.Build();

        build.Should().Throw<FluentValidation.ValidationException>()
             .WithMessage("*sequence must have a meaningful description*");
    }


    [Fact]
    public void Build_ShouldThrowValidationError_When_DescriptionIsEmpty()
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

        var build = () => builder.Build();

        build.Should().Throw<FluentValidation.ValidationException>()
             .WithMessage("*sequence must have a meaningful description*");
    }

    [Fact]
    public void Build_ShouldThrowValidationError_When_DescriptionIsNull()
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

        var build = () => builder.Build();

        build.Should().Throw<FluentValidation.ValidationException>()
             .WithMessage("*sequence must have a meaningful description*");
    }
}
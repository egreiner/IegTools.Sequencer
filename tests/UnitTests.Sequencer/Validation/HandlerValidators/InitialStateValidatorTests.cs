namespace UnitTests.Sequencer.Validation.HandlerValidators;

using IegTools.Sequencer.Validation;

public class InitialStateValidatorTests
{
    [Fact]
    public void Build_ShouldThrowValidationError_When_InitialStateIsMissing()
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.WithValidator<InitialStateValidator>();
        });

        var build = () => builder.Build();

        build.Should().Throw<FluentValidation.ValidationException>()
             .WithMessage("*Initial-State*")
             .WithMessage("*must be defined*");
    }

    [Fact]
    public void Build_ShouldThrowValidationError_When_StateTransitionsAreMissing()
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.WithValidator<InitialStateValidator>();

            builder.SetInitialState("State1");
        });

        var build = () => builder.Build();

        build.Should().Throw<FluentValidation.ValidationException>()
             .WithMessage("*Initial-State*")
             .WithMessage("*StateTransition*");
    }

    [Fact]
    public void Build_ShouldNotThrowValidationError_When_StateTransitionsAreValid()
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.WithValidator<InitialStateValidator>();

            builder.SetInitialState("State1");
            builder.AddTransition("State1", "State2", () => true);
            builder.AddTransition("State2", "State1", () => true);
        });

        var build = () => builder.Build();

        build.Should().NotThrow();
    }

    [Fact]
    public void Build_ShouldNotThrowValidationError_When_ContainsTransitionsAreValid()
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.WithValidator<InitialStateValidator>();

            builder.SetInitialState("State1");
            builder.AddTransition("State2", "State1", () => true);
            builder.AddContainsTransition("State", "State2", () => true);
        });

        var build = () => builder.Build();

        build.Should().NotThrow();
    }

    [Fact]
    public void Build_ShouldNotThrowValidationError_When_AnyTransitionsAreValid()
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.WithValidator<InitialStateValidator>();

            builder.SetInitialState("State1");
            builder.AddTransition("State1", "State2", () => true);
            builder.AddAnyTransition(new[] { "State1", "State2" }, "!State3", () => true);
        });

        var build = () => builder.Build();

        build.Should().NotThrow();
    }
}
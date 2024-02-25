namespace UnitTests.Sequencer.Validation.HandlerValidators;

using IegTools.Sequencer.Validation;

public class InitialStateValidatorTests
{
    [Fact]
    public void Should_throw_ValidationError_No_InitialState()
    {
        var builder = SequenceBuilder.Configure(builder
            =>
        {
            builder.WithValidator<InitialStateValidator>();
        });

        FluentActions.Invoking(() => builder.Build())
            .Should().Throw<FluentValidation.ValidationException>()
            .WithMessage("*Initial-State*")
            .WithMessage("*must be defined*");
    }

    [Fact]
    public void Should_throw_ValidationError_No_Transition()
    {
        var builder = SequenceBuilder.Configure(builder
            =>
        {
            builder.WithValidator<InitialStateValidator>();

            builder.SetInitialState("State1");
        });

        FluentActions.Invoking(() => builder.Build())
            .Should().Throw<FluentValidation.ValidationException>()
            .WithMessage("*Initial-State*")
            .WithMessage("*StateTransition*");
    }

    [Fact]
    public void Should_not_throw_ValidationError_Transition()
    {
        var builder = SequenceBuilder.Configure(builder
            =>
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
    public void Should_not_throw_ValidationError_ContainsTransition()
    {
        var builder = SequenceBuilder.Configure(builder
            =>
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
    public void Should_not_throw_ValidationError_AnyTransition()
    {
        var builder = SequenceBuilder.Configure(builder
            =>
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
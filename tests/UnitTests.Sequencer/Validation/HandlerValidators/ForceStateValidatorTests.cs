namespace UnitTests.Sequencer.Validation.HandlerValidators;

using IegTools.Sequencer.Validation;

public class ForceStateValidatorTests
{
    [Fact]
    public void Should_throw_ValidationError()
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.WithValidator<ForceStateValidator>();

            builder.SetInitialState("State1");
            builder.AddForceState("unknown", () => true);
        });

        var build = () => builder.Build();

        build.Should().Throw<FluentValidation.ValidationException>()
             .WithMessage("*Each Force-State*");
    }

    [Fact]
    public void Should_not_throw_ValidationError_Transition()
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.WithValidator<ForceStateValidator>();

            builder.SetInitialState("State1");
            builder.AddTransition("State1", "!State2", () => true);

            builder.AddForceState("State1", () => true);
        });

        var build = () => builder.Build();

        build.Should().NotThrow();
    }

    [Fact]
    public void Should_not_throw_ValidationError_ContainsTransition()
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.WithValidator<ForceStateValidator>();

            builder.SetInitialState("State1");
            builder.AddContainsTransition("State", "!State2", () => true);

            builder.AddForceState("State1", () => true);
        });

        var build = () => builder.Build();

        build.Should().NotThrow();
    }

    [Fact]
    public void Should_not_throw_ValidationError_AnyTransition()
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.WithValidator<ForceStateValidator>();

            builder.SetInitialState("State1");
            builder.AddAnyTransition(new[] { "State1", "!State2" }, "!State3", () => true);

            builder.AddForceState("State1", () => true);
        });

        var build = () => builder.Build();

        build.Should().NotThrow();
    }
}
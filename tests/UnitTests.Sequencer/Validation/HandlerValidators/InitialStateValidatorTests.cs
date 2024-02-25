namespace UnitTests.Sequencer.Validation.HandlerValidators;

public class InitialStateValidatorTests
{
    [Fact]
    public void Test_ThrowsValidationError_no_InitialState()
    {
        var builder = SequenceBuilder.Configure(builder
            =>
        {
            builder.WithValidator<IegTools.Sequencer.Validation.InitialStateValidator>();
        });

        FluentActions.Invoking(() => builder.Build())
            .Should().Throw<FluentValidation.ValidationException>()
            .WithMessage("*Initial-State*")
            .WithMessage("*must be defined*");
    }

    [Fact]
    public void Test_ThrowsValidationError_no_Transition()
    {
        var builder = SequenceBuilder.Configure(builder
            =>
        {
            builder.WithValidator<IegTools.Sequencer.Validation.InitialStateValidator>();
            builder.SetInitialState("State1");
        });

        FluentActions.Invoking(() => builder.Build())
            .Should().Throw<FluentValidation.ValidationException>()
            .WithMessage("*Initial-State*")
            .WithMessage("*StateTransition*");
    }

    [Fact]
    public void Test_Does_not_throw_ValidationError_Transition()
    {
        var builder = SequenceBuilder.Configure(builder
            =>
        {
            builder.SetInitialState("State1");
            builder.AddTransition("State1", "State2", () => true);
            builder.AddTransition("State2", "State1", () => true);
        });

        var build = () => builder.Build();
        build.Should().NotThrow();
    }

    [Fact]
    public void Test_Does_not_throw_ValidationError_ContainsTransition()
    {
        var builder = SequenceBuilder.Configure(builder
            =>
        {
            builder.SetInitialState("State1");
            builder.AddTransition("State2", "State1", () => true);
            builder.AddContainsTransition("State", "State2", () => true);
        });

        var build = () => builder.Build();
        build.Should().NotThrow();
    }

    [Fact]
    public void Test_Does_not_throw_ValidationError_AnyTransition()
    {
        var builder = SequenceBuilder.Configure(builder
            =>
        {
            builder.SetInitialState("State1");
            builder.AddTransition("State1", "State2", () => true);
            builder.AddAnyTransition(new[] { "State1", "State2" }, "!State3", () => true);
        });

        var build = () => builder.Build();
        build.Should().NotThrow();
    }
}
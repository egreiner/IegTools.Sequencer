namespace UnitTests.Sequencer.Validation;

public class InitialStateValidatorTests
{
    ////[Fact]
    ////public void Test_ThrowsValidationError_no_InitialState()
    ////{
    ////    var builder = SequenceBuilder.Configure(_ => {});

    ////    FluentActions.Invoking(() => builder.Build())
    ////        .Should().Throw<FluentValidation.ValidationException>()
    ////        .WithMessage("*The Initial-State*");
    ////}

    [Fact]
    public void Test_ThrowsValidationError_no_Transition()
    {
        var builder = SequenceBuilder.Configure(builder
            =>
        {
            builder.SetInitialState("State1");
        });

        FluentActions.Invoking(() => builder.Build())
            .Should().Throw<FluentValidation.ValidationException>()
            .WithMessage("*The Initial-State*");
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
            builder.AddTransition("State2", "State1", () => true);
            builder.AddAnyTransition(new[] { "State1", "!State2" }, "State3", () => true);
        });

        var build = () => builder.Build();
        build.Should().NotThrow();
    }
}
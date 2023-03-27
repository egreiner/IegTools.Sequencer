namespace UnitTests.Sequencer.Validation.RuleValidators;

public class ForceStateRuleValidatorTests
{
    [Fact]
    public void Test_ThrowsValidationError()
    {
        var builder = SequenceBuilder.Configure(builder
            =>
        {
            builder.SetInitialState("State1");
            builder.AddForceState("unknown", () => true);
        });

        FluentActions.Invoking(() => builder.Build())
            .Should().Throw<FluentValidation.ValidationException>()
            .WithMessage("*Each Force-State*");
    }

    [Fact]
    public void Test_Does_not_throw_ValidationError_Transition()
    {
        var builder = SequenceBuilder.Configure(builder
            =>
        {
            builder.SetInitialState("State1");
            builder.AddTransition("State1", "!State2", () => true);

            builder.AddForceState("State1", () => true);
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
            builder.AddContainsTransition("State", "!State2", () => true);

            builder.AddForceState("State1", () => true);
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
            builder.AddAnyTransition(new[] { "State1", "!State2" }, "!State3", () => true);

            builder.AddForceState("State1", () => true);
        });

        var build = () => builder.Build();
        build.Should().NotThrow();
    }

    ////[Theory]
    ////[InlineData(true)]
    ////[InlineData(false)]
    ////public void Test_ThrowsValidationError(bool constraint)
    ////{
    ////    var builder = SequenceBuilder.Configure(builder
    ////        =>
    ////    {
    ////        builder.SetInitialState("State1");
    ////        builder.AddForceState("unknown", () => true);
    ////    });

    ////    FluentActions.Invoking(() => builder.Build())
    ////        .Should().Throw<FluentValidation.ValidationException>()
    ////        .WithMessage("*Each Force-State*");
    ////}


}
namespace UnitTests.Sequencer.Validation.RuleValidators;

public class ContainsStateTransitionRuleValidatorTests
{
    [Fact]
    public void Test_ThrowsValidationError_wrong_FromState()
    {
        var builder = SequenceBuilder.Configure(builder
            =>
        {
            builder.SetInitialState("State1");
            builder.AddTransition("State1", "State2", () => true);
            builder.AddTransition("State2", "State1", () => true);

            builder.AddContainsTransition("StateX", "!State3", () => true);
        });

        FluentActions.Invoking(() => builder.Build())
            .Should().Throw<FluentValidation.ValidationException>()
            .WithMessage("*ContainsStateTransition*")
            .WithMessage("*Each 'State-part'*");
    }

    [Fact]
    public void Test_ThrowsValidationError_wrong_ToState()
    {
        var builder = SequenceBuilder.Configure(builder
            =>
        {
            builder.SetInitialState("State1");
            builder.AddTransition("State1", "State2", () => true);
            builder.AddTransition("State2", "State1", () => true);

            builder.AddContainsTransition("State", "not existing", () => true);
        });

        FluentActions.Invoking(() => builder.Build())
            .Should().Throw<FluentValidation.ValidationException>()
            .WithMessage("*ContainsStateTransition*")
            .WithMessage("*Each 'ToState'*");
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

            builder.AddContainsTransition("State", "!ActivatedByApi", () => true);
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
            builder.AddTransition("State1", "State2", () => true);
            builder.AddTransition("State2", "State1", () => true);

            builder.AddContainsTransition("State", "ActivatedByApi", () => true);

            builder.AddContainsTransition("ActivatedBy", "!State3", () => true);
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
            builder.AddTransition("State2", "State1", () => true);

            builder.AddContainsTransition("State", "ActivatedByApi", () => true);

            builder.AddAnyTransition(new[] { "State1", "ActivatedByApi" }, "!State4", () => true);
        });

        var build = () => builder.Build();
        build.Should().NotThrow();
    }
}
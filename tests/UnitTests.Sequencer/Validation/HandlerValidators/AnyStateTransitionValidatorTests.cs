namespace UnitTests.Sequencer.Validation.HandlerValidators;

using IegTools.Sequencer.Validation;

public class AnyStateTransitionValidatorTests
{
    [Fact]
    public void Should_throw_ValidationError_wrong_FromState()
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.WithValidator<AnyStateTransitionValidator>();

            builder.SetInitialState("State1");
            builder.AddTransition("State1", "State2", () => true);
            builder.AddTransition("State2", "State1", () => true);

            builder.AddAnyTransition(new[] { "StateX", "State2" }, "!State3", () => true);
        });

        var build = () => builder.Build();

        build.Should().Throw<FluentValidation.ValidationException>()
             .WithMessage("*AnyStateTransition*")
             .WithMessage("*Each 'FromState'*");
    }

    [Fact]
    public void Should_throw_ValidationError_wrong_ToState()
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.WithValidator<AnyStateTransitionValidator>();

            builder.SetInitialState("State1");
            builder.AddTransition("State1", "State2", () => true);
            builder.AddTransition("State2", "State1", () => true);

            builder.AddAnyTransition(new[] { "State1", "State2" }, "State3", () => true);
        });

        var build = () => builder.Build();

        build.Should().Throw<FluentValidation.ValidationException>()
             .WithMessage("*AnyStateTransition*")
             .WithMessage("*Each 'ToState'*");
    }

    [Fact]
    public void Should_not_throw_ValidationError_Transition()
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.WithValidator<AnyStateTransitionValidator>();

            builder.SetInitialState("State1");
            builder.AddTransition("State1", "State2", () => true);
            builder.AddTransition("State2", "State1", () => true);

            builder.AddAnyTransition(new[] { "State1", "State2" }, "!State3", () => true);
        });

        var build = () => builder.Build();

        build.Should().NotThrow();
    }

    [Fact]
    public void Should_not_throw_ValidationError_ContainsTransition()
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.WithValidator<AnyStateTransitionValidator>();

            builder.SetInitialState("State1");
            builder.AddTransition("State1", "State2", () => true);
            builder.AddTransition("State2", "State1", () => true);

            builder.AddAnyTransition(new[] { "State1", "State2" }, "State3", () => true);

            builder.AddContainsTransition("State", "State2", () => true);
        });

        var build = () => builder.Build();

        build.Should().NotThrow();
    }

    [Fact]
    public void Should_not_throw_ValidationError_AnyTransition()
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.WithValidator<AnyStateTransitionValidator>();

            builder.SetInitialState("State1");
            builder.AddTransition("State1", "State2", () => true);
            builder.AddTransition("State2", "State1", () => true);

            builder.AddAnyTransition(new[] { "State1", "State2" }, "State3", () => true);

            builder.AddAnyTransition(new[] { "State1", "State3" }, "!State4", () => true);
        });

        var build = () => builder.Build();

        build.Should().NotThrow();
    }
}
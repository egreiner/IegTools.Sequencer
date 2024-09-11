namespace UnitTests.Sequencer.Validation.HandlerValidators;

using IegTools.Sequencer.Validation;

public class AnyStateTransitionValidatorTests
{
    [Fact]
    public void Build_ShouldThrowValidationError_When_FromStateIsInvalid()
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
    public void Build_ShouldThrowValidationError_When_ToStateIsInvalid()
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
    public void Build_ShouldNotThrowValidationError_When_TransitionsAreValid()
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
    public void Build_ShouldNotThrowValidationError_When_ContainsTransitionsAreValid()
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.WithValidator<AnyStateTransitionValidator>();

            builder.SetInitialState("State1");
            builder.AddTransition("State1", "State2", () => true);
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
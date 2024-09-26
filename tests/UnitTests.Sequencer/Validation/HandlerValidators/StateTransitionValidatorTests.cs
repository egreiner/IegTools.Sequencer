namespace UnitTests.Sequencer.Validation.HandlerValidators;

using IegTools.Sequencer.Validation;

public class StateTransitionValidatorTests
{
    [Fact]
    public void Build_ShouldThrowValidationError_When_FromStateIsInvalid()
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.WithValidator<StateTransitionValidator>();

            builder.SetInitialState("State1");
            builder.AddTransition("State1", "!State2", () => true);
            builder.AddTransition("State3", "!State2", () => true);
        });

        var build = () => builder.Build();

        build.Should().Throw<FluentValidation.ValidationException>()
             .WithMessage("*StateTransition*")
             .WithMessage("*Each 'FromState'*");
    }

    [Fact]
    public void Build_ShouldThrowValidationError_When_ToStateIsInvalid()
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.WithValidator<StateTransitionValidator>();

            builder.SetInitialState("State1");
            builder.AddTransition("State1", "State2", () => true);
        });

        var build = () => builder.Build();

        build.Should().Throw<FluentValidation.ValidationException>()
             .WithMessage("*StateTransition*")
             .WithMessage("*Each 'ToState'*");
    }

    [Fact]
    public void Build_ShouldNotThrowValidationError_When_TransitionsAreValid()
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.WithValidator<StateTransitionValidator>();

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
            builder.WithValidator<StateTransitionValidator>();

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
            builder.WithValidator<StateTransitionValidator>();

            builder.SetInitialState("State1");
            builder.AddTransition("State1", "State2", () => true);
            builder.AddAnyTransition(new[] { "State1", "State2" }, "!State3", () => true);
        });

        var build = () => builder.Build();

        build.Should().NotThrow();
    }
}
namespace UnitTests.Sequencer.Validation.HandlerValidators;

using IegTools.Sequencer.Validation;

public class ContainsStateTransitionValidatorTests
{
    [Fact]
    public void Build_ShouldThrowValidationError_When_FromStateIsInvalid()
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.WithValidator<ContainsStateTransitionValidator>();

            builder.SetInitialState("State1");
            builder.AddTransition("State1", "State2", () => true);
            builder.AddTransition("State2", "State1", () => true);

            builder.AddContainsTransition("StateX", "!State3", () => true);
        });

        var build = () => builder.Build();

        build.Should().Throw<FluentValidation.ValidationException>()
             .WithMessage("*ContainsStateTransition*")
             .WithMessage("*Each 'State-part'*");
    }

    [Fact]
    public void Build_ShouldThrowValidationError_When_ToStateIsInvalid()
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.WithValidator<ContainsStateTransitionValidator>();

            builder.SetInitialState("State1");
            builder.AddTransition("State1", "State2", () => true);
            builder.AddTransition("State2", "State1", () => true);

            builder.AddContainsTransition("State", "not existing", () => true);
        });

        var build = () => builder.Build();

        build.Should().Throw<FluentValidation.ValidationException>()
             .WithMessage("*ContainsStateTransition*")
             .WithMessage("*Each 'ToState'*");
    }

    [Fact]
    public void Build_ShouldNotThrowValidationError_When_TransitionsAreValid()
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.WithValidator<ContainsStateTransitionValidator>();

            builder.SetInitialState("State1");
            builder.AddTransition("State1", "State2", () => true);
            builder.AddTransition("State2", "State1", () => true);

            builder.AddContainsTransition("State", "!ActivatedByApi", () => true);
        });

        var build = () => builder.Build();

        build.Should().NotThrow();
    }

    [Fact]
    public void Build_ShouldNotThrowValidationError_When_ContainsTransitionsAreValid()
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.WithValidator<ContainsStateTransitionValidator>();

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
    public void Build_ShouldNotThrowValidationError_When_AnyTransitionsAreValid()
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.WithValidator<ContainsStateTransitionValidator>();

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
namespace UnitTests.Sequencer.Validation.HandlerValidators;

using IegTools.Sequencer.Validation;

public class StateToggleValidatorTests
{
    [Fact]
    public void Should_throw_ValidationError_missing_FromState()
    {
        var builder = SequenceBuilder.Configure(builder
            =>
        {
            builder.WithValidator<StateToggleValidator>();

            builder.SetInitialState("State1");
            builder.AddStateToggle("State1", "State2", () => true, () => false);
            builder.AddTransition("State2", "State3", () => true);
        });

        FluentActions.Invoking(() => builder.Build())
            .Should().Throw<FluentValidation.ValidationException>()
            .WithMessage("*StateToggle*")
            .WithMessage("*missing 'FromState'*");
    }

    // [Fact]
    // public void Should_throw_ValidationError_wrong_ToState()
    // {
    //     var builder = SequenceBuilder.Configure(builder
    //         =>
    //     {
    //         builder.WithValidator<StateToggleValidator>();
    //
    //         builder.SetInitialState("State1");
    //         builder.AddTransition("State1", "State2", () => true);
    //     });
    //
    //     FluentActions.Invoking(() => builder.Build())
    //         .Should().Throw<FluentValidation.ValidationException>()
    //         .WithMessage("*StateTransition*")
    //         .WithMessage("*Each 'ToState'*");
    // }
    //
    // [Fact]
    // public void Should_not_throw_ValidationError_Transition()
    // {
    //     var builder = SequenceBuilder.Configure(builder
    //         =>
    //     {
    //         builder.WithValidator<StateToggleValidator>();
    //
    //         builder.SetInitialState("State1");
    //         builder.AddTransition("State1", "State2", () => true);
    //         builder.AddTransition("State2", "State1", () => true);
    //     });
    //
    //     var build = () => builder.Build();
    //     build.Should().NotThrow();
    // }
    //
    // [Fact]
    // public void Should_not_throw_ValidationError_ContainsTransition()
    // {
    //     var builder = SequenceBuilder.Configure(builder
    //         =>
    //     {
    //         builder.WithValidator<StateToggleValidator>();
    //
    //         builder.SetInitialState("State1");
    //         builder.AddTransition("State2", "State1", () => true);
    //
    //         builder.AddContainsTransition("State", "State2", () => true);
    //     });
    //
    //     var build = () => builder.Build();
    //     build.Should().NotThrow();
    // }
    //
    // [Fact]
    // public void Should_not_throw_ValidationError_AnyTransition()
    // {
    //     var builder = SequenceBuilder.Configure(builder
    //         =>
    //     {
    //         builder.WithValidator<StateToggleValidator>();
    //
    //         builder.SetInitialState("State1");
    //         builder.AddTransition("State1", "State2", () => true);
    //         builder.AddAnyTransition(new[] { "State1", "State2" }, "!State3", () => true);
    //     });
    //
    //     var build = () => builder.Build();
    //     build.Should().NotThrow();
    // }
}
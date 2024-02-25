namespace UnitTests.Sequencer.Validation;

using UnitTests.Sequencer.StateAsEnum;

public class ConfigurationValidatorEnumTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Should_throw_ValidationError_InitialStateEmpty(bool constraint)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            // builder.SetInitialState(TestEnum.InitialState)
            builder.AddTransition(TestEnum.State1, TestEnum.State2, () => constraint);
            builder.AddTransition(TestEnum.State2, TestEnum.State1, () => constraint);
        });

        FluentActions.Invoking(() => builder.Build())
            .Should().Throw<FluentValidation.ValidationException>()
            .WithMessage("*Initial-State*");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Should_throw_ValidationError_HandlerCount(bool constraint)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(TestEnum.State1);
            builder.DisableValidationForStates(TestEnum.State2);

            builder.AddTransition(TestEnum.State1, TestEnum.State2, () => constraint);
        });

        FluentActions.Invoking(() => builder.Build())
            .Should().Throw<FluentValidation.ValidationException>()
            .WithMessage("*more than one handler*");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Should_not_throw_ValidationError_HandlerCount(bool constraint)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(TestEnum.State1);
            builder.AddForceState(TestEnum.State1, () => constraint);
            builder.AddTransition(TestEnum.State1, TestEnum.State2, () => constraint);
            builder.AddTransition(TestEnum.State2, TestEnum.State1, () => constraint);
        });

        var build = () => builder.Build();
        build.Should().NotThrow();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Should_throw_ValidationError_wrong_NextState(bool constraint)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(TestEnum.InitialState);
            builder.AddTransition(TestEnum.State1, TestEnum.State2, () => constraint);
            builder.AddTransition(TestEnum.State2, TestEnum.NotExisting, () => constraint);
        });

        FluentActions.Invoking(() => builder.Build())
            .Should().Throw<FluentValidation.ValidationException>()
            .WithMessage("*Each 'ToState'*");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Should_throw_ValidationError_wrong_FromState(bool constraint)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(TestEnum.InitialState);

            builder.DisableValidationForStates(TestEnum.State2);

            builder.AddTransition(TestEnum.State1, TestEnum.State2, () => constraint);
            builder.AddTransition(TestEnum.NotExisting, TestEnum.State1, () => constraint);
        });

        FluentActions.Invoking(() => builder.Build())
            .Should().Throw<FluentValidation.ValidationException>()
            .WithMessage("*Each 'FromState'*");
    }


    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Should_throw_ValidationError_wrong_CurrentState2(bool constraint)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(TestEnum.InitialState);

            builder.DisableValidationForStates(TestEnum.State2);

            builder.AddForceState(TestEnum.State1, () => constraint);
            builder.AddTransition(TestEnum.State1, TestEnum.State2, () => constraint);
            builder.AddTransition(TestEnum.NotExisting, TestEnum.State1, () => constraint);
        });

        FluentActions.Invoking(() => builder.Build())
            .Should().Throw<FluentValidation.ValidationException>()
            .WithMessage("*Each 'FromState'*");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Should_not_throw_ValidationError(bool constraint)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(TestEnum.State1);

            builder.DisableValidationForStates(TestEnum.State2);

            builder.AddForceState(TestEnum.State3, () => constraint);
            builder.AddTransition(TestEnum.State1, TestEnum.State2, () => constraint);
            builder.AddTransition(TestEnum.State3, TestEnum.State1, () => constraint);
        });

        var build = () => builder.Build();
        build.Should().NotThrow();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Should_not_throw_ValidationError_MissingState(bool constraint)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(TestEnum.State1);

            builder.DisableValidationForStates(TestEnum.Unknown);

            builder.AddForceState(TestEnum.State1, () => constraint);
            builder.AddTransition(TestEnum.State1, TestEnum.State2, () => constraint);
            builder.AddTransition(TestEnum.State2, TestEnum.Unknown, () => constraint);
        });

        var build = () => builder.Build();
        build.Should().NotThrow();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Should_not_throw_ValidationError_MissingState_with_DeadEndCharacter(bool constraint)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(TestEnum.State1);

            builder.DisableValidationForStates(TestEnum.Unknown);

            builder.AddForceState(TestEnum.State1, () => constraint);
            builder.AddTransition(TestEnum.State1, TestEnum.State2, () => constraint);
            builder.AddTransition(TestEnum.State2, TestEnum.Unknown, () => constraint);
        });

        var build = () => builder.Build();
        build.Should().NotThrow();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Should_not_throw_ValidationError_HandlerState(bool constraint)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(TestEnum.State1);
            builder.AddForceState(TestEnum.State1, () => constraint);
            builder.AddTransition(TestEnum.State1, TestEnum.State2, () => constraint);
            builder.AddTransition(TestEnum.State2, TestEnum.State1, () => constraint);
        });

        var build = () => builder.Build();
        build.Should().NotThrow();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Should_not_throw_ValidationError_HandlerState2(bool constraint)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(TestEnum.State1);
            builder.AddTransition(TestEnum.State1, TestEnum.State2, () => constraint);
            builder.AddTransition(TestEnum.State2, TestEnum.State1, () => constraint);
        });

        var build = () => builder.Build();
        build.Should().NotThrow();
    }
}
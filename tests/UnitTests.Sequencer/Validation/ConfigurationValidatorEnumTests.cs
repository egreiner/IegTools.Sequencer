namespace UnitTests.Sequencer.Validation;

using UnitTests.Sequencer.StateAsEnum;

public class ConfigurationValidatorEnumTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_ThrowsValidationError_InitialStateEmpty(bool constraint)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            // builder.SetInitialState(TestEnum.InitialState)
            builder.AddTransition(TestEnum.State1, TestEnum.State2, () => constraint);
            builder.AddTransition(TestEnum.State2, TestEnum.State1, () => constraint);
        });

        FluentActions.Invoking(() => builder.Build())
            .Should().Throw<FluentValidation.ValidationException>()
            .WithMessage("*Initial State*")
            .Where(x => !x.Message.Contains("Each goto state"));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_ThrowsValidationError_RuleCount(bool constraint)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(TestEnum.InitialState);
        });

        FluentActions.Invoking(() => builder.Build())
            .Should().Throw<FluentValidation.ValidationException>()
            .WithMessage("*more than one rule*");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_DoesNotThrowValidationError_RuleCount(bool constraint)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(TestEnum.InitialState);
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
    public void Test_DoesThrowValidationError_wrong_NextState(bool constraint)
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
    public void Test_DoesThrowValidationError_wrong_FromState(bool constraint)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(TestEnum.InitialState);

            builder.DisableValidationForStatuses(TestEnum.State2);

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
    public void Test_DoesThrowValidationError_wrong_CurrentState2(bool constraint)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(TestEnum.InitialState);

            builder.DisableValidationForStatuses(TestEnum.State2);

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
    public void Test_DoesNotThrowValidationError(bool constraint)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(TestEnum.InitialState);

            builder.DisableValidationForStatuses(TestEnum.State2);

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
    public void Test_DoesNotThrowValidationError_MissingState(bool constraint)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(TestEnum.InitialState);

            builder.DisableValidationForStatuses(TestEnum.Unknown);

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
    public void Test_DoesNotThrowValidationError_MissingState_with_DeadEndCharacter(bool constraint)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(TestEnum.InitialState);

            builder.DisableValidationForStatuses(TestEnum.Unknown);

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
    public void Test_DoesNotThrowValidationError_RuleState(bool constraint)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(TestEnum.InitialState);
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
    public void Test_DoesNotThrowValidationError_RuleState2(bool constraint)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(TestEnum.InitialState);
            builder.AddTransition(TestEnum.State1, TestEnum.State2, () => constraint);
            builder.AddTransition(TestEnum.State2, TestEnum.State1, () => constraint);
        });

        var build = () => builder.Build();
        build.Should().NotThrow();
    }
}
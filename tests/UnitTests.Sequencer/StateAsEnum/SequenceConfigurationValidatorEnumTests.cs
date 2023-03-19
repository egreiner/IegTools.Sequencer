using IegTools.Sequencer.Extensions;

namespace UnitTests.Sequencer.StateAsEnum;

public class SequenceConfigurationValidatorEnumTests
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

        var actual = Assert.Throws<FluentValidation.ValidationException>(() => builder.Build());

        actual.Message.Should().Contain("Initial State");
        actual.Message.Should().NotContain("Each goto state");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_ThrowsValidationError_DescriptorCount(bool constraint)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(TestEnum.InitialState);
        });

        var actual = Assert.Throws<FluentValidation.ValidationException>(() => builder.Build());

        actual.Message.Should().Contain("Descriptors Count");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_DoesNotThrowValidationError_DescriptorCount(bool constraint)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(TestEnum.InitialState);
            builder.AddForceState(TestEnum.State1, () => constraint);
            builder.AddTransition(TestEnum.State1, TestEnum.State2, () => constraint);
            builder.AddTransition(TestEnum.State2, TestEnum.State1, () => constraint);
        });

        var sut = builder.Build();

        sut.Should().NotBeNull();
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

        var actual = Assert.Throws<FluentValidation.ValidationException>(() => builder.Build());

        actual.Message.Should().Contain("Each 'ToState'");
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

        var actual = Assert.Throws<FluentValidation.ValidationException>(() => builder.Build());

        actual.Message.Should().Contain("Each 'FromState'");
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

        var actual = Assert.Throws<FluentValidation.ValidationException>(() => builder.Build());

        actual.Message.Should().Contain("Each 'FromState'");
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

        var sut = builder.Build();

        sut.Should().NotBeNull();
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

        var sut = builder.Build();

        sut.Should().NotBeNull();
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

        var sut = builder.Build();

        sut.Should().NotBeNull();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_DoesNotThrowValidationError_DescriptorState(bool constraint)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(TestEnum.InitialState);
            builder.AddForceState(TestEnum.State1, () => constraint);
            builder.AddTransition(TestEnum.State1, TestEnum.State2, () => constraint);
            builder.AddTransition(TestEnum.State2, TestEnum.State1, () => constraint);
        });

        var sut = builder.Build();

        sut.Should().NotBeNull();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_DoesNotThrowValidationError_DescriptorState2(bool constraint)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(TestEnum.InitialState);
            builder.AddTransition(TestEnum.State1, TestEnum.State2, () => constraint);
            builder.AddTransition(TestEnum.State2, TestEnum.State1, () => constraint);
        });

        var sut = builder.Build();

        sut.Should().NotBeNull();
    }
}
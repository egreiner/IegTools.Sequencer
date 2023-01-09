namespace UnitTests.Sequencer;

public class SequenceConfigurationValidatorEnumTests
{
    private enum MyEnum
    {
        InitialState,
        State1,
        State2,
        State3,
        NotExisting,
        Unknown,
        Force
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_ThrowsValidationError_InitialStateEmpty(bool constraint)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            // builder.SetInitialState(MyEnum.InitialState)
            builder.AddTransition(MyEnum.State1, MyEnum.State2, () => constraint);
            builder.AddTransition(MyEnum.State2, MyEnum.State1, () => constraint);
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
            builder.SetInitialState(MyEnum.InitialState);
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
            builder.SetInitialState(MyEnum.InitialState);
            builder.AddForceState(MyEnum.State1, () => constraint);
            builder.AddTransition(MyEnum.State1, MyEnum.State2, () => constraint);
            builder.AddTransition(MyEnum.State2, MyEnum.State1, () => constraint);
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
            builder.SetInitialState(MyEnum.InitialState);
            builder.AddTransition(MyEnum.State1, MyEnum.State2, () => constraint);
            builder.AddTransition(MyEnum.State2, MyEnum.NotExisting, () => constraint);
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
            builder.SetInitialState(MyEnum.InitialState);
            
            builder.DisableValidationForStatuses(MyEnum.State2);
            
            builder.AddTransition(MyEnum.State1, MyEnum.State2, () => constraint);
            builder.AddTransition(MyEnum.NotExisting, MyEnum.State1, () => constraint);
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
            builder.SetInitialState(MyEnum.InitialState);
            
            builder.DisableValidationForStatuses(MyEnum.State2);
            
            builder.AddForceState(MyEnum.State1, () => constraint);
            builder.AddTransition(MyEnum.State1, MyEnum.State2, () => constraint);
            builder.AddTransition(MyEnum.NotExisting, MyEnum.State1, () => constraint);
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
            builder.SetInitialState(MyEnum.InitialState);

            builder.DisableValidationForStatuses(MyEnum.State2);
            
            builder.AddForceState(MyEnum.State3, () => constraint);
            builder.AddTransition(MyEnum.State1, MyEnum.State2, () => constraint);
            builder.AddTransition(MyEnum.State3, MyEnum.State1, () => constraint);
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
            builder.SetInitialState(MyEnum.InitialState);

            builder.DisableValidationForStatuses(MyEnum.Unknown);
            
            builder.AddForceState(MyEnum.State1, () => constraint);
            builder.AddTransition(MyEnum.State1, MyEnum.State2, () => constraint);
            builder.AddTransition(MyEnum.State2, (MyEnum.Unknown), () => constraint);
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
            builder.SetInitialState(MyEnum.InitialState);
            
            builder.DisableValidationForStatuses(MyEnum.Unknown);
            
            builder.AddForceState(MyEnum.State1, () => constraint);
            builder.AddTransition(MyEnum.State1, MyEnum.State2, () => constraint);
            builder.AddTransition(MyEnum.State2, MyEnum.Unknown, () => constraint);
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
            builder.SetInitialState(MyEnum.InitialState);
            builder.AddForceState(MyEnum.State1, () => constraint);
            builder.AddTransition(MyEnum.State1, MyEnum.State2, () => constraint);
            builder.AddTransition(MyEnum.State2, MyEnum.State1, () => constraint);
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
            builder.SetInitialState(MyEnum.InitialState);
            builder.AddTransition(MyEnum.State1, MyEnum.State2, () => constraint);
            builder.AddTransition(MyEnum.State2, MyEnum.State1, () => constraint);
        });

        var sut = builder.Build();

        sut.Should().NotBeNull();
    }
}
namespace UnitTests.Sequencer;

using FluentAssertions;

// TODO clean up
public class SequenceConfigurationValidatorTests
{
    private const string InitialState = "InitialState";

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_ThrowsValidationError_InitialStateEmpty(bool constraint)
    {
        var countStarts = 0;
        var builder = SequenceBuilder.Configure(builder =>
        {
            // builder.SetInitialState(InitialState)
            builder.AddTransition("State1", "State2", () => constraint, () => countStarts++);
            builder.AddTransition("State2", "State1", () => constraint, () => countStarts++);
        });

        var actual = Assert.Throws<FluentValidation.ValidationException>(() => builder.Build());

        actual.Message.Should().Contain("Initial State");
        actual.Message.Should().NotContain("Each goto state");
        countStarts.Should().Be(0);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_ThrowsValidationError_DescriptorCount(bool constraint)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(InitialState);
            builder.AddForceState("Force", () => constraint);
        });

        var actual = Assert.Throws<FluentValidation.ValidationException>(() => builder.Build());

        actual.Message.Should().Contain("Descriptors Count");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_DoesNotThrowValidationError_DescriptorCount(bool constraint)
    {
        var countStarts = 0;
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(InitialState);
            builder.AddForceState("State1", () => constraint);
            builder.AddTransition("State1", "State2", () => constraint, () => countStarts++);
            builder.AddTransition("State2", "State1", () => constraint, () => countStarts++);
        });

        var sut = builder.Build();

        sut.Should().NotBeNull();
        countStarts.Should().Be(0);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_DoesThrowValidationError_wrong_NextState(bool constraint)
    {
        var countStarts = 0;
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(InitialState);
            builder.AddTransition("State1", "State2", () => constraint, () => countStarts++);
            builder.AddTransition("State2", "not existing", () => constraint, () => countStarts++);
        });

        var actual = Assert.Throws<FluentValidation.ValidationException>(() => builder.Build());

        actual.Message.Should().Contain("Each 'ToState'");
        countStarts.Should().Be(0);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_DoesThrowValidationError_wrong_CurrentState(bool constraint)
    {
        var countStarts = 0;
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(InitialState);
            builder.AddTransition("State1", "!State2", () => constraint, () => countStarts++);
            builder.AddTransition("not existing", "State1", () => constraint, () => countStarts++);
        });

        var actual = Assert.Throws<FluentValidation.ValidationException>(() => builder.Build());

        actual.Message.Should().Contain("Each 'FromState'");
        countStarts.Should().Be(0);
    }
    

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_DoesThrowValidationError_wrong_CurrentState2(bool constraint)
    {
        var countStarts = 0;
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(InitialState);
            builder.AddForceState("State1", () => constraint);
            builder.AddTransition("State1", "!State2", () => constraint, () => countStarts++);
            builder.AddTransition("not existing", "State1", () => constraint, () => countStarts++);
        });

        var actual = Assert.Throws<FluentValidation.ValidationException>(() => builder.Build());

        actual.Message.Should().Contain("Each 'FromState'");
        countStarts.Should().Be(0);
    }
    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_DoesNotThrowValidationError_wrong_CurrentState(bool constraint)
    {
        var countStarts = 0;
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(InitialState);
            builder.AddForceState("State3", () => constraint);
            builder.AddTransition("State1", "!State2", () => constraint, () => countStarts++);
            builder.AddTransition("State3", "State1", () => constraint, () => countStarts++);
        });

        var sut = builder.Build();

        sut.Should().NotBeNull();
        countStarts.Should().Be(0);
    }
    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_DoesNotThrowValidationError_MissingState(bool constraint)
    {
        var countStarts = 0;
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(InitialState);
            builder.AddForceState("State1", () => constraint);
            builder.AddTransition("State1", "State2", () => constraint, () => countStarts++);
            builder.AddTransition("State2", "unknown", () => constraint, () => countStarts++);
            builder.DisableValidationForStatuses("unknown", "unknown1", "unknown2");
        });

        var sut = builder.Build();

        sut.Should().NotBeNull();
        countStarts.Should().Be(0);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_DoesNotThrowValidationError_MissingState_with_DeadEndCharacter(bool constraint)
    {
        var countStarts = 0;
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(InitialState);
            builder.AddForceState("State1", () => constraint);
            builder.AddTransition("State1", "State2", () => constraint, () => countStarts++);
            // '!' is the DeadEndCharacter
            builder.AddTransition("State2", "!unknown", () => constraint, () => countStarts++);
        });

        var sut = builder.Build();

        sut.Should().NotBeNull();
        countStarts.Should().Be(0);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_DoesNotThrowValidationError_DescriptorState(bool constraint)
    {
        var countStarts = 0;
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(InitialState);
            builder.AddForceState("State1", () => constraint);
            builder.AddTransition("State1", "State2", () => constraint, () => countStarts++);
            builder.AddTransition("State2", "State1", () => constraint, () => countStarts++);
        });

        var sut = builder.Build();

        sut.Should().NotBeNull();
        countStarts.Should().Be(0);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_DoesNotThrowValidationError_DescriptorState2(bool constraint)
    {
        var countStarts = 0;
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(InitialState);
            builder.AddTransition("State1", "State2", () => constraint, () => countStarts++);
            builder.AddTransition("State2", "State1", () => constraint, () => countStarts++);
        });

        var sut = builder.Build();

        sut.Should().NotBeNull();
        countStarts.Should().Be(0);
    }
}
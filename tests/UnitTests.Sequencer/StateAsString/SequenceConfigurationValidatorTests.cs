using IegTools.Sequencer.Extensions;

namespace UnitTests.Sequencer.StateAsString;

public class SequenceConfigurationValidatorTests
{
    private const string InitialState = "InitialState";

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_ThrowsValidationError_InitialStateEmpty(bool constraint)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            // builder.SetInitialState(InitialState)
            builder.AddTransition("State1", "State2", () => constraint);
            builder.AddTransition("State2", "State1", () => constraint);
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
            builder.SetInitialState(InitialState);
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
            builder.SetInitialState(InitialState);
            builder.AddForceState("State1", () => constraint);
            builder.AddTransition("State1", "State2", () => constraint);
            builder.AddTransition("State2", "State1", () => constraint);
        });

        var sut = builder.Build();

        sut.Should().NotBeNull();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_DoesNotThrowValidationError_WrongNextState(bool constraint)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState("State1");
            builder.AddTransition("State1", "State2", () => constraint);
            builder.AddTransition("State2", "State1", () => !constraint);
        });

        var sut = builder.Build();

        sut.Should().NotBeNull();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_DoesThrowValidationError_WrongNextState(bool constraint)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(InitialState);
            builder.AddTransition("State1", "State2", () => constraint);
            builder.AddTransition("State2", "State1", () => constraint);
            builder.AddTransition("State2", "not existing", () => constraint);
        });

        var actual = Assert.Throws<FluentValidation.ValidationException>(() => builder.Build());

        actual.Message.Should().Contain("Each 'ToState'");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_DoesThrowValidationError_WrongCurrentState(bool constraint)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(InitialState);
            builder.AddTransition("State1", "!State2", () => constraint);
            builder.AddTransition("not existing", "State1", () => constraint);
        });

        var actual = Assert.Throws<FluentValidation.ValidationException>(() => builder.Build());

        actual.Message.Should().Contain("Each 'FromState'");
    }


    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_DoesThrowValidationError_WrongCurrentState2(bool constraint)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(InitialState);
            builder.AddForceState("State1", () => constraint);
            builder.AddTransition("State1", "!State2", () => constraint);
            builder.AddTransition("not existing", "State1", () => constraint);
        });

        var actual = Assert.Throws<FluentValidation.ValidationException>(() => builder.Build());

        actual.Message.Should().Contain("Each 'FromState'");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_DoesNotThrowValidationError_WrongCurrentState(bool constraint)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(InitialState);
            builder.AddForceState("State3", () => constraint);
            builder.AddTransition("State1", "!State2", () => constraint);
            builder.AddTransition("State3", "State1", () => constraint);
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
            builder.SetInitialState(InitialState);

            builder.DisableValidationForStatuses("unknown", "unknown1", "unknown2");

            builder.AddForceState("State1", () => constraint);
            builder.AddTransition("State1", "State2", () => constraint);
            builder.AddTransition("State2", "unknown", () => constraint);
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
            builder.SetInitialState(InitialState);
            builder.AddForceState("State1", () => constraint);
            builder.AddTransition("State1", "State2", () => constraint);

            // '!' is the DeadEndCharacter
            builder.AddTransition("State2", "!unknown", () => constraint);
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
            builder.SetInitialState(InitialState);
            builder.AddForceState("State1", () => constraint);
            builder.AddTransition("State1", "State2", () => constraint);
            builder.AddTransition("State2", "State1", () => constraint);
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
            builder.SetInitialState(InitialState);
            builder.AddTransition("State1", "State2", () => constraint);
            builder.AddTransition("State2", "State1", () => constraint);
        });

        var sut = builder.Build();

        sut.Should().NotBeNull();
    }
}
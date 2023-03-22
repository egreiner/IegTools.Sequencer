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

        FluentActions.Invoking(() => builder.Build())
            .Should().Throw<FluentValidation.ValidationException>()
            .WithMessage("*Initial State*")
            .Where(x => !x.Message.Contains("Each goto state"));
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

        FluentActions.Invoking(() => builder.Build())
            .Should().Throw<FluentValidation.ValidationException>()
            .WithMessage("*Descriptors Count*");
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

        var build = () => builder.Build();
        build.Should().NotThrow();
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

        var build = () => builder.Build();
        build.Should().NotThrow();
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

        FluentActions.Invoking(() => builder.Build())
            .Should().Throw<FluentValidation.ValidationException>()
            .WithMessage("*Each 'ToState'*");
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

        FluentActions.Invoking(() => builder.Build())
            .Should().Throw<FluentValidation.ValidationException>()
            .WithMessage("*Each 'FromState'*");
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

        FluentActions.Invoking(() => builder.Build())
            .Should().Throw<FluentValidation.ValidationException>()
            .WithMessage("*Each 'FromState'*");
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
            builder.SetInitialState(InitialState);

            builder.DisableValidationForStatuses("unknown", "unknown1", "unknown2");

            builder.AddForceState("State1", () => constraint);
            builder.AddTransition("State1", "State2", () => constraint);
            builder.AddTransition("State2", "unknown", () => constraint);
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
            builder.SetInitialState(InitialState);
            builder.AddForceState("State1", () => constraint);
            builder.AddTransition("State1", "State2", () => constraint);

            // '!' is the DeadEndCharacter
            builder.AddTransition("State2", "!unknown", () => constraint);
        });

        var build = () => builder.Build();
        build.Should().NotThrow();
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

        var build = () => builder.Build();
        build.Should().NotThrow();
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

        var build = () => builder.Build();
        build.Should().NotThrow();
    }
}
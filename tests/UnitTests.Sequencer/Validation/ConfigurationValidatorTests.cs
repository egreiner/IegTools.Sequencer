namespace UnitTests.Sequencer.Validation;

public class ConfigurationValidatorTests
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
                .WithMessage("*InitialState*");
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Test_ThrowsValidationError_HandlerCount(bool constraint)
        {
            var builder = SequenceBuilder.Configure(builder =>
            {
                builder.SetInitialState(InitialState);
                builder.AddTransition(InitialState, "!State2", () => constraint);
            });

            FluentActions.Invoking(() => builder.Build())
                .Should().Throw<FluentValidation.ValidationException>()
                .WithMessage("*more than one handler*");
        }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_DoesNotThrowValidationError_HandlerCount(bool constraint)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(InitialState);
            builder.AddForceState("State1", () => constraint);
            builder.AddTransition("InitialState", "State1", () => constraint);
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

            builder.AddTransition(InitialState, "State1", () => constraint);
            builder.AddTransition("State1", "State2", () => constraint);
            builder.AddTransition("State2", "State1", () => constraint);
            builder.AddTransition("State2", "not existing", () => constraint);
            builder.AddTransition("State2", "not existing2", () => constraint);
        });

        FluentActions.Invoking(() => builder.Build())
            .Should().Throw<FluentValidation.ValidationException>()
            .WithMessage("*not existing*")
            .WithMessage("*not existing2*");
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

            builder.AddTransition("InitialState", "State1", () => constraint);
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

            builder.DisableValidationForStates("unknown", "unknown1", "unknown2");

            builder.AddForceState("State1", () => constraint);

            builder.AddTransition("InitialState", "State1", () => constraint);
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
            builder.AddTransition("InitialState", "State1", () => constraint);
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
    public void Test_DoesNotThrowValidationError_HandlerState(bool constraint)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(InitialState);
            builder.AddForceState("State1", () => constraint);
            builder.AddTransition("InitialState", "State1", () => constraint);
            builder.AddTransition("State1", "State2", () => constraint);
            builder.AddTransition("State2", "State1", () => constraint);
        });

        var build = () => builder.Build();
        build.Should().NotThrow();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_DoesNotThrowValidationError_HandlerState2(bool constraint)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(InitialState);
            builder.AddTransition("InitialState", "State1", () => constraint);
            builder.AddTransition("State1", "State2", () => constraint);
            builder.AddTransition("State2", "State1", () => constraint);
        });

        var build = () => builder.Build();
        build.Should().NotThrow();
    }
}
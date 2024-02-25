namespace UnitTests.Sequencer.Validation;

public class ConfigurationValidatorTests
    {
        private const string InitialState = "InitialState";

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Should_throw_ValidationError_InitialStateEmpty(bool constraint)
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
        public void Should_throw_ValidationError_HandlerCount(bool constraint)
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
    public void Should_not_throw_ValidationError_HandlerCount(bool constraint)
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
    public void Should_not_throw_ValidationError_WrongNextState(bool constraint)
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
    public void Should_throw_ValidationError_WrongNextState(bool constraint)
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
    public void Should_throw_ValidationError_WrongCurrentState(bool constraint)
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
    public void Should_throw_ValidationError_WrongCurrentState2(bool constraint)
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
    public void Should_not_throw_ValidationError_WrongCurrentState(bool constraint)
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
    public void Should_not_throw_ValidationError_MissingState(bool constraint)
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
    public void Should_not_throw_ValidationError_MissingState_with_DeadEndCharacter(bool constraint)
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
    public void Should_not_throw_ValidationError_HandlerState(bool constraint)
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
    public void Should_not_throw_ValidationError_HandlerState2(bool constraint)
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
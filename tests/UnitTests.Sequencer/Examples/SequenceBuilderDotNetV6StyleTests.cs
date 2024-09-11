namespace UnitTests.Sequencer.Examples;

using IegTools.Sequencer;

public class SequenceBuilderDotNetV6StyleTests
{
    private const string InitialState = "InitialState";

    private readonly DefaultSequenceStates _state = new DefaultSequenceStates();
    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Example_ValidFluentConfiguration_ShouldNotThrowValidationException(bool constraint)
    {
        var builder = SequenceBuilder.Create()
            .SetInitialState(InitialState)
            .AddForceState("Force", () => constraint)
            .DisableValidation();

        var build = () => builder.Build();

        build.Should().NotThrow<FluentValidation.ValidationException>();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Example_ValidNonFluentConfiguration_ShouldNotThrowValidationException(bool constraint)
    {
        var builder = SequenceBuilder.Create();
        builder.SetInitialState(InitialState);
        builder.AddForceState("Force", () => constraint);
        builder.DisableValidation();

        var build = () => builder.Build();

        build.Should().NotThrow<FluentValidation.ValidationException>();
    }


    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Example_InvalidConfiguration_ShouldThrowValidationException(bool constraint)
    {
        var builder = SequenceBuilder.Create()
            .SetInitialState(InitialState)
            .AddForceState("Force", () => constraint);
            //.DisableValidation();

            var build = () => builder.Build();

            build.Should().Throw<FluentValidation.ValidationException>();
    }



    [Fact]
    public void Example_ValidFluentConfiguration_for_OnTimer()
    {
        ISequence sequence = null;

        var input = true;

        var builder = SequenceBuilder.Create()
            .AddForceState(_state.Off, () => !input)
            .AddTransition(_state.Off, _state.WaitOn, () => input, () => sequence?.Stopwatch.Restart())
            .AddTransition(_state.WaitOn, _state.On, () => input && sequence?.Stopwatch.ElapsedMilliseconds > 1000);

        builder.SetInitialState(_state.Off)
               .DisableValidationForStates(_state.On);

        var build = () => sequence = builder.Build();

        build.Should().NotThrow<FluentValidation.ValidationException>();
    }

    [Fact]
    public void Example_ValidFluentConfiguration_for_OffTimer()
    {
        var result = 0;
        var builder = SequenceBuilder.Create()
            .SetInitialState("!Off")
            .AddForceState("On", () => false)
            .AddTransition("On", "PrepareOff", () => false, () => result = 1)
            .AddTransition("PrepareOff", "!Off", () => false);

        var build = () => builder.Build();

        build.Should().NotThrow<FluentValidation.ValidationException>();
        result.Should().Be(0);
    }

    [Fact]
    public void Example_ValidFluentConfiguration_for_PulseTimer()
    {
        var result = 0;
        var builder = SequenceBuilder.Create()
            .AddTransition(">Off", "PrepareOn", () => false)
            .AddTransition("PrepareOn", "Pulse", () => true, () => result = 1)
            .AddTransition("Pulse", "PrepareOff", () => false)
            .AddTransition("PrepareOff", ">Off", () => false);

        var build = () => builder.Build();

        build.Should().NotThrow<FluentValidation.ValidationException>();
        result.Should().Be(0);
    }
}
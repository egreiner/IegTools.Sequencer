namespace UnitTests.Sequencer.Examples;

using FluentValidation;

public class SequenceBuilderDotNetV5StyleTests
{
    private const string InitialState = "InitialState";


    [Fact]
    public void Example_configure_a_AntiStickingFeature()
    {
        var result = 0;
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState("Paused");

            builder.AddForceState("Paused", () => false);

            builder.AddTransition("Paused", "Activated", () => false, () => result = 1);
            builder.AddTransition("Activated", "Pump on", () => true, () => result++);
            builder.AddTransition("Pump on", "Pump off", () => false, () => result++);
            builder.AddTransition("Pump off", "Pump on", () => false);
            builder.AddTransition("Pump off", "Paused", () => false, () => result++);
        });

        var build = () => builder.Build();

        build.Should().NotThrow();
        result.Should().Be(0);
    }
    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Example_ValidConfiguration_ShouldNotThrowValidationException(bool constraint)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(InitialState);
            builder.AddForceState("Force", () => constraint);
            builder.DisableValidation();
        });

        var build = () => builder.Build();

        build.Should().NotThrow();
    }
    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Example_InvalidConfiguration_ShouldThrowValidationException(bool constraint)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(InitialState);
            builder.AddForceState("Force", () => constraint);
            //builder.DisableValidation();
        });

        var build = () => builder.Build();

        build.Should().Throw<ValidationException>();
    }
}
namespace UnitTests.Sequencer.Examples;

using FluentValidation;
using IegTools.Sequencer.Extensions;

public class SequenceBuilderConfigureTests
{
    private const string InitialState = "InitialState";


    [Fact]
    public void Test_AntiStickingFeature()
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

        var sut = builder.Build();

        sut.Should().NotBeNull();
        result.Should().Be(0);
    }
    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_Configure_NET5(bool constraint)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(InitialState);
            builder.AddForceState("Force", () => constraint);
            builder.DisableValidation();
        });

        var actual = builder.Build();

        actual.Should().NotBeNull();
    }
    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_Configure_throws_ValidationException(bool constraint)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(InitialState);
            builder.AddForceState("Force", () => constraint);
            //builder.DisableValidation();
        });

        Assert.Throws<ValidationException>(() => _ = builder.Build());
    }
}
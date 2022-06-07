namespace UnitTests.Sequencer;

using FluentAssertions;

public class RealWorldSequenceConfigurationTests
{
    [Fact]
    public void Test_OnTimer()
    {
        var result = 0;
        var builder = SequenceBuilder.Create()
            .AddForceState(">Off", () => false)
            .AddTransition(">Off", "PrepareOn", () => false, () => result = 1)
            .AddTransition("PrepareOn", "!On", () => false);

        var sut = builder.Build();

        sut.Should().NotBeNull();
        result.Should().Be(0);
    }

    [Fact]
    public void Test_OffTimer()
    {
        var result = 0;
        var builder = SequenceBuilder.Create()
            .SetInitialState("!Off")
            .AddForceState("On", () => false)
            .AddTransition("On", "PrepareOff", () => false, () => result = 1)
            .AddTransition("PrepareOff", "!Off", () => false);

        var sut = builder.Build();

        sut.Should().NotBeNull();
        result.Should().Be(0);
    }
    
    [Fact]
    public void Test_PulseTimer()
    {
        var result = 0;
        var builder = SequenceBuilder.Create()
            .AddTransition(">Off", "PrepareOn", () => false)
            .AddTransition("PrepareOn", "Pulse", () => true, () => result = 1)
            .AddTransition("Pulse", "PrepareOff", () => false)
            .AddTransition("PrepareOff", ">Off", () => false);

        var sut = builder.Build();

        sut.Should().NotBeNull();
        result.Should().Be(0);
    }
    
    [Fact]
    public void Test_AntiStickingFeature()
    {
        var result = 0;
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState("Paused");

            builder.AddForceState("Paused", () => false);
            
            builder.AddTransition("Paused", "Activated",  () => false, () => result = 1);
            builder.AddTransition("Activated", "Pump on", () => true,  () => result++);
            builder.AddTransition("Pump on", "Pump off",  () => false, () => result++);
            builder.AddTransition("Pump off", "Pump on",  () => false); 
            builder.AddTransition("Pump off", "Paused",   () => false, () => result++);
        });

        var sut = builder.Build();

        sut.Should().NotBeNull();
        result.Should().Be(0);
    }
}
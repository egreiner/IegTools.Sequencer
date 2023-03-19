namespace UnitTests.Sequencer.Examples;

using FluentValidation;
using IegTools.Sequencer.Extensions;

public class SequenceBuilderCreateTests
{
    private const string InitialState = "InitialState";

    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_Create_NET6v1(bool constraint)
    {
        var builder = SequenceBuilder.Create()
            .SetInitialState(InitialState)
            .AddForceState("Force", () => constraint)
            .DisableValidation();

        var actual = builder.Build();

        actual.Should().NotBeNull();
    }
    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_Create_throws_ValidationException(bool constraint)
    {
        var builder = SequenceBuilder.Create()
            .SetInitialState(InitialState)
            .AddForceState("Force", () => constraint);
            //.DisableValidation();

            Assert.Throws<ValidationException>(() => _ = builder.Build());
    }

    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_Create_NET6v2(bool constraint)
    {
        var builder = SequenceBuilder.Create();
        builder.SetInitialState(InitialState);
        builder.AddForceState("Force", () => constraint);
        builder.DisableValidation();

        var actual = builder.Build();

        actual.Should().NotBeNull();
    }


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
}
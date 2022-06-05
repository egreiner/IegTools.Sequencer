namespace UnitTests.Sequencer;

using FluentAssertions;

public class SequenceConfigurationTests
{
    private const string InitialState = "InitialState";

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
    public void Test_Create_NET6v2(bool constraint)
    {
        var builder = SequenceBuilder.Create();
        builder.SetInitialState(InitialState);
        builder.AddForceState("Force", () => constraint);
        builder.DisableValidation();

        var actual = builder.Build();

        actual.Should().NotBeNull();
    }
}
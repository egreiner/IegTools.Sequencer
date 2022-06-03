namespace UnitTests.Sequencer;

using FluentAssertions;

public class SequenceConfigurationTests
{
    private const string InitialState = "InitialState";

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_Configure(bool constraint)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.AddForceState("Force", () => constraint);
            builder.DisableValidation();
        });

        var actual = builder.Build(InitialState);

        actual.Should().NotBeNull();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_Create(bool constraint)
    {
        var builder = SequenceBuilder.Create()
            .AddForceState("Force", () => constraint)
            .DisableValidation();

        var actual = builder.Build(InitialState);

        actual.Should().NotBeNull();
    }
}
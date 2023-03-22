using IegTools.Sequencer.Images;

namespace UnitTests.Sequencer.StateAsString;

public class InitialStateTagTests
{
    private const string InitialState = ">InitialState";

    [Theory]
    [InlineData(true, ">Force")]
    [InlineData(false, InitialState)]
    public void Test_ForceStateDescriptor(bool constraint, string expected)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.AddForceState(expected, () => constraint)
                .DisableValidation();
        });

        builder.Configuration.InitialState.Should().Be(expected);
    }

    [Theory]
    [InlineData(true, ">Force")]
    [InlineData(false, InitialState)]
    public void Test_AddTransitionDescriptor(bool constraint, string expected)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.AddTransition(expected, "State2", () => constraint)
                .DisableValidation();
        });

        builder.Configuration.InitialState.Should().Be(expected);
    }

    [Theory]
    [InlineData(">Force")]
    [InlineData(InitialState)]
    public void Test_AddStateActionDescriptor(string expected)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.AddStateAction(expected, null)
                .DisableValidation();
        });

        builder.Configuration.InitialState.Should().Be(expected);
    }
}
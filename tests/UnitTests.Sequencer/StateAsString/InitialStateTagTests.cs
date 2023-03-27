namespace UnitTests.Sequencer.StateAsString;

public class InitialStateTagTests
{
    private const string InitialState = ">InitialState";

    [Theory]
    [InlineData(true, ">Force")]
    [InlineData(false, InitialState)]
    public void Test_ForceStateRule(bool constraint, string expected)
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
    public void Test_AddTransitionRule(bool constraint, string expected)
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
    public void Test_AddStateActionRule(string expected)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.AddStateAction(expected, null)
                .DisableValidation();
        });

        builder.Configuration.InitialState.Should().Be(expected);
    }
}
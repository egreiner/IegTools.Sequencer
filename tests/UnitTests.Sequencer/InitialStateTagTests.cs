namespace UnitTests.Sequencer;

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

        var actual = builder.Configuration.InitialState;
        Assert.Equal(expected, actual);
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

        var actual = builder.Configuration.InitialState;
        Assert.Equal(expected, actual);
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

        var actual = builder.Configuration.InitialState;
        Assert.Equal(expected, actual);
    }
}
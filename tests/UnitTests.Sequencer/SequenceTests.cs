namespace UnitTests.Sequencer;

public class SequenceTests
{
    private const string InitialState = "InitialState";

    [Theory]
    [InlineData(true, "Force")]
    [InlineData(false, InitialState)]
    public void Test_ForceState(bool constraint, string expected)
    {
        var builder = SequenceBuilder.Configure(InitialState, builder =>
            builder.AddForceState("Force", () => constraint));

        var sut = builder.Build(InitialState).Run();

        var actual = sut.CurrentState;
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(true, "Force")]
    [InlineData(false, InitialState)]
    public void Test_ForceState_Only_Last_Counts(bool constraint, string expected)
    {
        var builder = SequenceBuilder.Configure(InitialState, builder =>
        {
            builder.AddForceState("test", () => constraint)
                .AddForceState("Force", () => constraint);
        });

        var sut = builder.Build(InitialState).Run();

        var actual = sut.CurrentState;
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(true, "Set")]
    [InlineData(false, InitialState)]
    public void Test_Set(bool constraint, string expected)
    {
        var builder = SequenceBuilder.Configure(InitialState, builder =>
            builder.AddForceState("Force", () => constraint));

        var sut = builder.Build(InitialState);

        sut.SetState("Set", () => constraint);

        // no Execute is necessary

        var actual = sut.CurrentState;
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(true, "Set")]
    [InlineData(false, InitialState)]
    public void Test_SetState_Only_Last_Counts(bool constraint, string expected)
    {
        var builder = SequenceBuilder.Configure(InitialState, builder =>
            builder.AddForceState("Force", () => constraint));

        var sut = builder.Build(InitialState);
            
        sut.SetState("test", () => constraint);
        sut.SetState("Set", () => constraint);

        // no Execute is necessary

        var actual = sut.CurrentState;
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("State1", true, "State2")]
    [InlineData("State1", false, "State1")]
    [InlineData("StateX", true, "StateX")]
    [InlineData("StateX", false, "StateX")]
    public void Test_Constrain_Add_Conditional_State(string currentState, bool constraint, string expected)
    {
        var builder = SequenceBuilder.Configure(InitialState, builder =>
            builder.AddTransition("State1", "State2", () => constraint));

        var sut = builder.Build(InitialState);

        sut.SetState(currentState);
        sut.Run();

        var actual = sut.CurrentState;
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("State1", true, 1)]
    [InlineData("State1", false, 0)]
    [InlineData("StateX", true, 0)]
    [InlineData("StateX", false, 0)]
    public void Test_Action_Add_Conditional_State(string currentState, bool constraint, int expected)
    {
        var countStarts = 0;
        var builder = SequenceBuilder.Configure(InitialState, builder =>
            builder.AddTransition("State1", "State2", () => constraint, () => countStarts = 1));

        var sut = builder.Build(InitialState);

        sut.SetState(currentState);
        sut.Run();

        var actual = countStarts;
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("State1", true, 2)]
    public void Test_Concatenation_Add_Conditional_State(string currentState, bool constraint, int expected)
    {
        var countStarts = 0;
        var builder = SequenceBuilder.Configure(InitialState, builder =>
        {
            builder.AddTransition("State1", "State2", () => constraint, () => countStarts++);
            builder.AddTransition("State2", "State3", () => constraint, () => countStarts++);
        });

        var sut = builder.Build(InitialState);

        sut.SetState(currentState);
        sut.Run();

        var actualCount = countStarts;
        Assert.Equal(expected, actualCount);

        var actualState = sut.CurrentState;
        Assert.Equal("State3", actualState);
    }
}
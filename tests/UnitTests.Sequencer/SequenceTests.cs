namespace UnitTests.Sequencer;

public class SequenceTests
{
    private const string InitialState = "InitialState";
    
    [Theory]
    [InlineData(true, "Force")]
    [InlineData(false, InitialState)]
    public void Test_ForceState(bool constraint, string expected)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(InitialState);
            builder.AddForceState("Force", () => constraint)
                .DisableValidation();
        });


        var sut = builder.Build().Run();

        var actual = sut.CurrentState;
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(true,  "Force1")]
    [InlineData(false, "Force2")]
    public void Test_AllForceStatuses_are_working(bool constraint, string expected)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(InitialState)
                .AddForceState("Force1", () => constraint)
                .AddForceState("Force2", () => !constraint)
                .DisableValidation();
        });

        var sut = builder.Build().Run();

        var actual = sut.CurrentState;
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(true, "Set")]
    [InlineData(false, InitialState)]
    public void Test_Set(bool constraint, string expected)
    {
        var builder = SequenceBuilder.Configure(builder =>
            builder.SetInitialState(InitialState)
                .AddForceState("Force", () => constraint)
                .DisableValidation());

        var sut = builder.Build();

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
        var builder = SequenceBuilder.Configure(builder =>
            builder.SetInitialState(InitialState)
                .AddForceState("Force", () => constraint)
                .DisableValidation());

        var sut = builder.Build();
            
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
        var builder = SequenceBuilder.Configure(builder =>
            builder.AddTransition("State1", "State2", () => constraint)
                .DisableValidation());

        var sut = builder.Build();

        sut.SetState(currentState);
        sut.Run();

        var actual = sut.CurrentState;
        Assert.Equal(expected, actual);
    }


    [Theory]
    [InlineData("State1", "State2", false)]
    [InlineData("State1", "State1", true)]
    public void Test_HasCurrentState(string currentState, string queryState, bool expected)
    {
        var builder = SequenceBuilder.Configure(builder =>
            builder.AddTransition("State1", "State2", () => false)
                .DisableValidation());

        var sut = builder.Build();

        sut.SetState(currentState);
        sut.Run();

        var actual = sut.HasCurrentState(queryState);
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
        var builder = SequenceBuilder.Configure(builder =>
            builder.AddTransition("State1", "State2", () => constraint, () => countStarts = 1)
                   .DisableValidation());

        var sut = builder.Build();

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
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(InitialState);
            builder.AddTransition("State1", "State2", () => constraint, () => countStarts++);
            builder.AddTransition("State2", "State3", () => constraint, () => countStarts++)
                   .DisableValidation();
        });

        var sut = builder.Build();

        sut.SetState(currentState);
        sut.Run();

        var actualCount = countStarts;
        Assert.Equal(expected, actualCount);

        var actualState = sut.CurrentState;
        Assert.Equal("State3", actualState);
    }

    [Theory]
    [InlineData(">State1", ">State2", 0)]
    [InlineData(">State1", ">State1", 1)]
    public void Test_AddStateActionDescriptor(string state, string currentState, int expected)
    {
        var result = 0;
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.AddStateAction(state, () => result++)
                .DisableValidation();
        });

        var sut = builder.Build();

        sut.SetState(currentState);
        sut.Run();

        var actual = result;
        Assert.Equal(expected, actual);
    }
}
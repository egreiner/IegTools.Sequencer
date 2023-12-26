namespace UnitTests.Sequencer.StateAsString;

public class StateTransitionHandlerTests
{
    private const string InitialState = "InitialState";


    [Theory]
    [InlineData("State1", true, "State2")]
    [InlineData("State1", false, "State1")]
    [InlineData("StateX", true, "State2")]
    [InlineData("StateX", false, "State1")]
    public void Test_Constrain_Add_Conditional_State(string currentState, bool constraint, string expected)
    {
        var sut = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState("State1");
            builder.AddTransition("State1", "State2", () => constraint)
                .DisableValidation();
        }).Build();

        sut.SetState(currentState);
        sut.Run();

        sut.CurrentState.Should().Be(expected);
    }

    [Theory]
    [InlineData("State1", true, "State2")]
    [InlineData("State1", false, "State1")]
    [InlineData("StateX", true, "State2")]
    [InlineData("StateX", false, "State1")]
    public void Test_Constrain_Add_Conditional_State_with_Title(string currentState, bool constraint, string expected)
    {
        var sut = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState("State1");
            builder.AddTransition("Test description", "State1", "State2", () => constraint)
                .DisableValidation();
        }).Build();

        sut.SetState(currentState);
        sut.Run();

        sut.CurrentState.Should().Be(expected);
    }


    [Theory]
    [InlineData("State1", "State2", false)]
    [InlineData("State1", "State1", true)]
    public void Test_HasCurrentState(string currentState, string queryState, bool expected)
    {
        var sut = SequenceBuilder.Configure(builder =>
            builder.SetInitialState("State1")
                .AddTransition("State1", "State2", () => false)
                .DisableValidation()).Build();

        sut.SetState(currentState);
        sut.Run();

        sut.HasCurrentState(queryState).Should().Be(expected);
    }

    [Theory]
    [InlineData("State1", false, false, "State1")]
    [InlineData("State1", false, true, "State1")]
    [InlineData("State2", false, false, "State1")]
    [InlineData("State2", false, true, "State2")]
    [InlineData("State1", true, false, "State1")]
    [InlineData("State1", true, true, "State2")]
    [InlineData("State2", true, false, "State1")]
    [InlineData("State2", true, true, "State2")]
    public void Test_LastState(string currentState, bool constraint1, bool constraint2, string expected)
    {
        var sut = SequenceBuilder.Configure(builder =>
            builder.SetInitialState("State1")
                .AddTransition("State1", "State2", () => constraint1)
                .AddTransition("State2", "State1", () => constraint2)
                .DisableValidation()).Build();

        sut.SetState(currentState);
        sut.Run();

        sut.LastState.Value.Should().Be(expected);
    }

    [Theory]
    [InlineData("State1", true, 1)]
    [InlineData("State1", false, 0)]
    [InlineData("StateX", true, 0)]
    [InlineData("StateX", false, 0)]
    public void Test_Action_Add_Conditional_State(string currentState, bool constraint, int expected)
    {
        var countStarts = 0;
        var sut = SequenceBuilder.Configure(builder =>
            builder.AddTransition("State1", "State2", () => constraint, () => countStarts = 1)
                .DisableValidation()).Build();

        sut.SetState(currentState);
        sut.Run();

        countStarts.Should().Be(expected);
    }

    [Theory]
    [InlineData("State1", true, 2)]
    public void Test_Concatenation_Add_Conditional_State(string currentState, bool constraint, int expected)
    {
        var countStarts = 0;
        var sut = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(InitialState);
            builder.AddTransition("State1", "State2", () => constraint, () => countStarts++);
            builder.AddTransition("State2", "State3", () => constraint, () => countStarts++)
                .DisableValidation();
        }).Build();

        sut.SetState(currentState);
        sut.Run();

        countStarts.Should().Be(expected);

        sut.CurrentState.Should().Be("State3");
    }

    [Theory]
    [InlineData("State1", true, "State2")]
    [InlineData("State1", false, "State1")]
    [InlineData("StateX", true, "State2")]
    [InlineData("StateX", false, "State1")]
    public async void Test_RunAsync(string currentState, bool constraint, string expected)
    {
        var sut = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState("State1");
            builder.AddTransition("State1", "State2", () => constraint)
                .DisableValidation();
        }).Build();

        sut.SetState(currentState);
        await sut.RunAsync();

        sut.CurrentState.Should().Be(expected);
    }
}
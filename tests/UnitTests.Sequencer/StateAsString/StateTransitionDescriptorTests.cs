using IegTools.Sequencer.Images;

namespace UnitTests.Sequencer.StateAsString;

public class StateTransitionDescriptorTests
{
    private const string InitialState = "InitialState";


    [Theory]
    [InlineData("State1", true, "State2")]
    [InlineData("State1", false, "State1")]
    [InlineData("StateX", true, "State2")]
    [InlineData("StateX", false, "State1")]
    public void Test_Constrain_Add_Conditional_State(string currentState, bool constraint, string expected)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState("State1");
            builder.AddTransition("State1", "State2", () => constraint)
                .DisableValidation();
        });

        var sut = builder.Build();

        sut.SetState(currentState);
        sut.Run();

        sut.CurrentState.Should().Be(expected);
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

        sut.HasCurrentState(queryState).Should().Be(expected);
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

        countStarts.Should().Be(expected);
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

        countStarts.Should().Be(expected);

        sut.CurrentState.Should().Be("State3");
    }
}
namespace UnitTests.Sequencer;

public class SequenceTests
{
    [Theory]
    [InlineData("State1", true)]
    [InlineData("State2", true)]
    [InlineData("State3", true)]
    [InlineData("NotDefined", false)]
    public void Test_IsRegisteredState(string state, bool expected)
    {
        var countStarts = 0;
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState("State1");
            builder.AddStateAction("State1", () => countStarts++);
            builder.AddTransition("State1", "State2", () => true, () => countStarts++);
            builder.AddTransition("State2", "State3", () => true, () => countStarts++)
                .DisableValidation();
        });

        var sut = builder.Build();

        var actual = sut.IsRegisteredState(state);
        Assert.Equal(expected, actual);
    }

        
    [Theory]
    [InlineData(false, "State1", "State2")]
    [InlineData(false, "State1", "State2", "InitialState", "Force")]
    [InlineData(true, "State1", "State1", "InitialState", "Force")]
    public void Test_HasAnyCurrentState(bool expected, string currentState, params string[] queryStates)
    {
        var builder = SequenceBuilder.Configure(builder =>
            builder.AddTransition("State1", "State2", () => false)
                .DisableValidation());

        var sut = builder.Build();

        sut.SetState(currentState);
        sut.Run();

        var actual = sut.HasAnyCurrentState(queryStates);
        Assert.Equal(expected, actual);
    }
}
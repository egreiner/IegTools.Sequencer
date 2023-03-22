using IegTools.Sequencer.Images;

namespace UnitTests.Sequencer.StateAsString;

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

        sut.IsRegisteredState(state).Should().Be(expected);
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

        sut.HasAnyCurrentState(queryStates).Should().Be(expected);
    }
}
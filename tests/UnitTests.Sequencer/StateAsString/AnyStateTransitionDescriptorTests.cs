namespace UnitTests.Sequencer.StateAsString;

using IegTools.Sequencer.Images;

public class AnyStateTransitionDescriptorTests
{
    [Theory]
    [InlineData("State1", "State1", true, "State", "State2")]
    [InlineData("State1", "State1", false, "State", "State2")]
    [InlineData("State1", "StateX", true, "State1", "State2")]
    [InlineData("State1", "State1", false, "State1", "State2")]

    [InlineData("State2", "StateX", true, "State", "State2")]
    [InlineData("State2", "State2", false, "State", "State2")]
    [InlineData("State2", "StateX", true, "State1", "State2")]
    [InlineData("State2", "State2", false, "State1", "State2")]

    [InlineData("State3", "State3", true, "State", "State2")]
    [InlineData("State3", "State3", false, "State", "State2")]
    [InlineData("State3", "State3", true, "State1", "State2")]
    [InlineData("State3", "State3", false, "State1", "State2")]
    public void Test_AddAnyTransition_more_compareState(string currentState, string expected, bool constraint, params string[] currentStateContains)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState("State1");
            
            builder.AddTransition("State1", "State2", () => false);
            builder.AddTransition("State2", "State3", () => false);
            
            builder.AddAnyTransition(currentStateContains, "StateX", () => constraint);

            builder.DisableValidation();
        });

        var sut = builder.Build();

        sut.SetState(currentState);
        sut.Run();

        sut.CurrentState.Should().Be(expected);
    }
}
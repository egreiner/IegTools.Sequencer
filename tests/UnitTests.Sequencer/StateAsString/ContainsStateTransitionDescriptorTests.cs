namespace UnitTests.Sequencer.StateAsString;

using IegTools.Sequencer.Extensions;

public class ContainsStateTransitionDescriptorTests
{
    [Theory]
    [InlineData("State1", true, "StateX")]
    [InlineData("State1", false, "State1")]
    [InlineData("State2", true, "StateX")]
    [InlineData("State2", false, "State2")]
    [InlineData("State3", true, "StateX")]
    [InlineData("State3", false, "State3")]
    [InlineData("StateX", true, "StateX")]
    [InlineData("StateX", false, "StateX")]
    public void Test_AddContainsTransition_one_compareState(string currentState, bool constraint, string expected)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState("State1");
            
            builder.AddTransition("State1", "State2", () => false);
            builder.AddTransition("State2", "State3", () => false);
            
            builder.AddContainsTransition("State", "StateX", () => constraint);

            builder.DisableValidation();
        });

        var sut = builder.Build();

        sut.SetState(currentState);
        sut.Run();

        sut.CurrentState.Should().Be(expected);
    }
}
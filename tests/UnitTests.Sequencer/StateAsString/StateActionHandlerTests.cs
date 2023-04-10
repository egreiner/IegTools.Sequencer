namespace UnitTests.Sequencer.StateAsString;

public class StateActionHandlerTests
{

    [Theory]
    [InlineData(">State1", ">State2", 0)]
    [InlineData(">State1", ">State1", 1)]
    public void Test_AddStateActionHandler(string state, string currentState, int expected)
    {
        var result = 0;
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.AddStateAction(state, () => result++);
            builder.AddStateAction(">State2", () => result = 0)
                .DisableValidation();
        });

        var sut = builder.Build();

        sut.SetState(currentState);
        sut.Run();

        result.Should().Be(expected);
    }
}
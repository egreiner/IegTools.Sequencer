namespace UnitTests.Sequencer.StateAsString;

public class StateActionHandlerTests
{
    [Theory]
    [InlineData(">State1", "State2", 10)]
    [InlineData(">State1", ">State1", 1)]
    public void Test_AddStateActionHandler(string state, string currentState, int expected)
    {
        var result = 0;
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.AddStateAction(state,     () => result++);
            builder.AddStateAction(">State2", () => result = 10)
                .DisableValidation();
        });

        var sut = builder.Build();

        sut.SetState(currentState);
        sut.Run();

        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(">State1", "State2", 10)]
    [InlineData(">State1", ">State1", 3)]
    public void Test_run_StateAction_multiple_times(string state, string currentState, int expected)
    {
        var result = 0;
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.AddStateAction(state,    () => result++);
            builder.AddStateAction("State2", () => result = 10)
                .DisableValidation();
        });

        var sut = builder.Build();

        sut.SetState(currentState);

        for (int i = 0; i < 3; i++)
        {
            sut.Run();
        }

        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(">State1", true,  01)]
    [InlineData(">State1", false, 00)]
    [InlineData(">State2", true,  10)]
    [InlineData(">State2", false, 00)]
    public void Test_AddStateActionHandler_with_Condition(string state, bool condition, int expected)
    {
        var result = 0;
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.AddStateAction(">State1", () => result = 01, () => condition);
            builder.AddStateAction(">State2", () => result = 10, () => condition)
                .DisableValidation();
        });

        var sut = builder.Build();

        sut.SetState(state);
        sut.Run();

        result.Should().Be(expected);
    }
}
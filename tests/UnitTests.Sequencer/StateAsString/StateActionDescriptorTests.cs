namespace UnitTests.Sequencer.StateAsString;

public class StateActionDescriptorTests
{

    [Theory]
    [InlineData(">State1", ">State2", 0)]
    [InlineData(">State1", ">State1", 1)]
    public void Test_AddStateActionDescriptor(string state, string currentState, int expected)
    {
        var result = 0;
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.AddStateAction(">State1", () => result++);
            builder.AddStateAction(">State2", () => result = 0)
                .DisableValidation();
        });

        var sut = builder.Build();

        sut.SetState(currentState);
        sut.Run();

        result.Should().Be(expected);
    }
}
namespace UnitTests.Sequencer.StateAsString;

public class SequenceIsRegisteredStateTests
{
    private const string InitialState = "InitialState";


    [Theory]
    [InlineData("Force", true)]
    [InlineData("NotDefined", false)]
    public void Test_IsRegisteredState(string state, bool expected)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(InitialState);
            builder.AddForceState("Force", () => true)
                .DisableValidation();
        });


        var sut = builder.Build();

        sut.IsRegisteredState(state).Should().Be(expected);
    }
}
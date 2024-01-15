namespace UnitTests.Sequencer.StateAsString;

public class ToggleStatesHandlerTests
{
    private const string InitialState = "InitialState";

    [Theory]
    [InlineData(false,false, InitialState)]
    [InlineData(false, true, InitialState)]
    [InlineData(true, false, "SetState")]
    public void Test_AddToggleStates(bool setCondition, bool resetCondition, string expectedState)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(InitialState);
            // builder.AddToggleStates(resetState: InitialState, setState: "SetState", dominantSetCondition: () => setCondition, resetCondition: () => resetCondition);
            builder.AddToggleStates(
                    resetState: InitialState,
                    setState: "SetState",
                    dominantSetCondition: () => setCondition,
                    resetCondition:       () => resetCondition)
                .DisableValidation();
        });

        var sut = builder.Build().Run();
        sut.Run();

        sut.CurrentState.Should().Be(expectedState);
    }
}
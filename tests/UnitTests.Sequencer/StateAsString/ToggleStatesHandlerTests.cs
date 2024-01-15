namespace UnitTests.Sequencer.StateAsString;

public class ToggleStatesHandlerTests
{
    private const string InitialState = "InitialState";

    [Theory]
    [InlineData(false,false, InitialState)]
    [InlineData(false, true, InitialState)]
    [InlineData(true, false, "SetState")]
    [InlineData(true, true,  "SetState")] // dominant set condition
    public void Test_AddToggleStates(bool setCondition, bool resetCondition, string expectedState)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(InitialState);
            builder.AddToggleStates(
                    resetState: InitialState,
                    setState: "SetState",
                    dominantSetCondition: () => setCondition,
                    resetCondition:       () => resetCondition)
                .DisableValidation();
        });

        var sut = builder.Build();
        sut.Run();

        sut.CurrentState.Should().Be(expectedState);
    }


    [Theory]
    [InlineData(false,false, InitialState, InitialState)]
    [InlineData(false, true, InitialState, InitialState)]
    [InlineData(true, false, InitialState, "SetState")]
    [InlineData(true, true,  InitialState, "SetState")] // dominant set condition
    [InlineData(false,false, "SetState", "SetState")]
    [InlineData(false, true, "SetState", InitialState)]
    [InlineData(true, false, "SetState", "SetState")]
    [InlineData(true, true,  "SetState", "SetState")] // dominant set condition
    public void Test_AddToggleStates2(bool setCondition, bool resetCondition, string setToState, string expectedState)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(InitialState);
            builder.AddToggleStates(
                    resetState: InitialState,
                    setState: "SetState",
                    dominantSetCondition: () => setCondition,
                    resetCondition:       () => resetCondition)
                .DisableValidation();
        });

        var sut = builder.Build();
        sut.SetState(setToState);
        sut.Run();

        sut.CurrentState.Should().Be(expectedState);
    }


    [Theory]
    [InlineData(false,false, InitialState, 0, 0)]
    [InlineData(false, true, InitialState, 0, 0)]
    [InlineData(true, false, InitialState, 1, 0)]
    [InlineData(true, true,  InitialState, 1, 0)] // dominant set condition
    [InlineData(false,false, "SetState",   0, 0)]
    [InlineData(false, true, "SetState",   0, 1)]
    [InlineData(true, false, "SetState",   0, 0)]
    [InlineData(true, true,  "SetState",   0, 0)] // dominant set condition
    public void Test_AddToggleStates_Actions(bool setCondition, bool resetCondition, string setToState, int expectedSetActionResult, int expectedResetActionResult)
    {
        var setActionCalled   = 0;
        var resetActionCalled = 0;

        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(InitialState);
            builder.AddToggleStates(
                    resetState: InitialState,
                    setState: "SetState",
                    dominantSetCondition: () => setCondition,
                    resetCondition:       () => resetCondition,
                    setAction:            () => setActionCalled++,
                    resetAction:          () => resetActionCalled++)
                .DisableValidation();
        });

        var sut = builder.Build();
        sut.SetState(setToState);
        sut.Run();

        setActionCalled.Should().Be(expectedSetActionResult);
        resetActionCalled.Should().Be(expectedResetActionResult);
    }
    [Theory]
    [InlineData(false,false, InitialState, 0, 0)]
    [InlineData(false, true, InitialState, 0, 0)]
    [InlineData(true, false, InitialState, 1, 0)]
    [InlineData(true, true,  InitialState, 1, 0)] // dominant set condition
    [InlineData(false,false, "SetState",   0, 0)]
    [InlineData(false, true, "SetState",   0, 1)]
    [InlineData(true, false, "SetState",   0, 0)]
    [InlineData(true, true,  "SetState",   0, 0)] // dominant set condition
    public void Test_AddToggleStates_Actions_run_twice(bool setCondition, bool resetCondition, string setToState, int expectedSetActionResult, int expectedResetActionResult)
    {
        var setActionCalled   = 0;
        var resetActionCalled = 0;

        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(InitialState);
            builder.AddToggleStates(
                    resetState: InitialState,
                    setState: "SetState",
                    dominantSetCondition: () => setCondition,
                    resetCondition:       () => resetCondition,
                    setAction:            () => setActionCalled++,
                    resetAction:          () => resetActionCalled++)
                .DisableValidation();
        });

        var sut = builder.Build();
        sut.SetState(setToState);
        sut.Run();
        sut.Run();

        setActionCalled.Should().Be(expectedSetActionResult);
        resetActionCalled.Should().Be(expectedResetActionResult);
    }
}
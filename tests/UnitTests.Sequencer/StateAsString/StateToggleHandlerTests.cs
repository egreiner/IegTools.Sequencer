namespace UnitTests.Sequencer.StateAsString;

public class StateToggleHandlerTests
{
    private const string InitialState = "InitialState";

    [Theory]
    [InlineData(false,false, InitialState)]
    [InlineData(false, true, InitialState)]
    [InlineData(true, false, "SetState")]
    [InlineData(true, true,  "SetState")] // dominant set condition
    public void Test_AddToggleStates(bool setToCondition, bool setFromCondition, string expectedState)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(InitialState);
            builder.AddStateToggle(
                    fromState: InitialState,
                    toState: "SetState",
                    dominantSetToCondition: () => setToCondition,
                    setFromCondition:       () => setFromCondition)
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
    public void Test_AddToggleStates2(bool setToCondition, bool setFromCondition, string setToState, string expectedState)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(InitialState);
            builder.AddStateToggle(
                    fromState: InitialState,
                    toState: "SetState",
                    dominantSetToCondition: () => setToCondition,
                    setFromCondition:       () => setFromCondition)
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
    public void Test_AddToggleStates_Actions(bool setToCondition, bool setFromCondition, string setToState, int expectedSetToActionResult, int expectedSetFromActionResult)
    {
        var setActionCalled   = 0;
        var resetActionCalled = 0;

        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(InitialState);
            builder.AddStateToggle(
                    fromState: InitialState,
                    toState: "SetState",
                    dominantSetToCondition: () => setToCondition,
                    setFromCondition:       () => setFromCondition,
                    setToAction:            () => setActionCalled++,
                    setFromAction:          () => resetActionCalled++)
                .DisableValidation();
        });

        var sut = builder.Build();
        sut.SetState(setToState);
        sut.Run();

        setActionCalled.Should().Be(expectedSetToActionResult);
        resetActionCalled.Should().Be(expectedSetFromActionResult);
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
    public void Test_AddToggleStates_Actions_run_twice(bool setToCondition, bool setFromCondition, string setToState, int expectedSetToActionResult, int expectedSetFromActionResult)
    {
        var setActionCalled   = 0;
        var resetActionCalled = 0;

        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(InitialState);
            builder.AddStateToggle(
                    fromState: InitialState,
                    toState: "SetState",
                    dominantSetToCondition: () => setToCondition,
                    setFromCondition:       () => setFromCondition,
                    setToAction:            () => setActionCalled++,
                    setFromAction:          () => resetActionCalled++)
                .DisableValidation();
        });

        var sut = builder.Build();
        sut.SetState(setToState);
        sut.Run();
        sut.Run();

        setActionCalled.Should().Be(expectedSetToActionResult);
        resetActionCalled.Should().Be(expectedSetFromActionResult);
    }
}
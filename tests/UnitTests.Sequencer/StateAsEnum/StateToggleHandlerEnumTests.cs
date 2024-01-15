namespace UnitTests.Sequencer.StateAsEnum;

public class StateToggleHandlerEnumTests
{
    private const TestEnum InitialState = TestEnum.InitialState;

    [Theory]
    [InlineData(false,false, InitialState)]
    [InlineData(false, true, InitialState)]
    [InlineData(true, false, TestEnum.State1)]
    [InlineData(true, true,  TestEnum.State1)] // dominant set condition
    public void Test_AddToggleStates(bool setToCondition, bool setFromCondition, TestEnum expectedState)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(InitialState);
            builder.AddStateToggle(
                    fromState: InitialState,
                    toState: TestEnum.State1,
                    dominantSetToCondition: () => setToCondition,
                    setFromCondition:       () => setFromCondition)
                .DisableValidation();
        });

        var sut = builder.Build();
        sut.Run();

        sut.CurrentState.Should().Be(expectedState.ToString());
    }


    [Theory]
    [InlineData(false,false, InitialState, InitialState)]
    [InlineData(false, true, InitialState, InitialState)]
    [InlineData(true, false, InitialState, TestEnum.State1)]
    [InlineData(true, true,  InitialState, TestEnum.State1)] // dominant set condition
    [InlineData(false,false, TestEnum.State1, TestEnum.State1)]
    [InlineData(false, true, TestEnum.State1, InitialState)]
    [InlineData(true, false, TestEnum.State1, TestEnum.State1)]
    [InlineData(true, true,  TestEnum.State1, TestEnum.State1)] // dominant set condition
    public void Test_AddToggleStates2(bool setToCondition, bool setFromCondition, TestEnum setToState, TestEnum expectedState)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(InitialState);
            builder.AddStateToggle(
                    fromState: InitialState,
                    toState: TestEnum.State1,
                    dominantSetToCondition: () => setToCondition,
                    setFromCondition:       () => setFromCondition)
                .DisableValidation();
        });

        var sut = builder.Build();
        sut.SetState(setToState);
        sut.Run();

        sut.CurrentState.Should().Be(expectedState.ToString());
    }


    [Theory]
    [InlineData(false,false, InitialState, 0, 0)]
    [InlineData(false, true, InitialState, 0, 0)]
    [InlineData(true, false, InitialState, 1, 0)]
    [InlineData(true, true,  InitialState, 1, 0)] // dominant set condition
    [InlineData(false,false, TestEnum.State1,   0, 0)]
    [InlineData(false, true, TestEnum.State1,   0, 1)]
    [InlineData(true, false, TestEnum.State1,   0, 0)]
    [InlineData(true, true,  TestEnum.State1,   0, 0)] // dominant set condition
    public void Test_AddToggleStates_Actions(bool setToCondition, bool setFromCondition, TestEnum setToState, int expectedSetToActionResult, int expectedSetFromActionResult)
    {
        var setActionCalled   = 0;
        var resetActionCalled = 0;

        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(InitialState);
            builder.AddStateToggle(
                    fromState: InitialState,
                    toState: TestEnum.State1,
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
    [InlineData(false,false, TestEnum.State1,   0, 0)]
    [InlineData(false, true, TestEnum.State1,   0, 1)]
    [InlineData(true, false, TestEnum.State1,   0, 0)]
    [InlineData(true, true,  TestEnum.State1,   0, 0)] // dominant set condition
    public void Test_AddToggleStates_Actions_run_twice(bool setToCondition, bool setFromCondition, TestEnum setToState, int expectedSetToActionResult, int expectedSetFromActionResult)
    {
        var setActionCalled   = 0;
        var resetActionCalled = 0;

        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(InitialState);
            builder.AddStateToggle(
                    fromState: InitialState,
                    toState: TestEnum.State1,
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
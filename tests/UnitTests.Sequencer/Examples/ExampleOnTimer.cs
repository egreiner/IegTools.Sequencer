namespace UnitTests.Sequencer.Examples;

using IegTools.Sequencer;

public class ExampleOnTimer
{
    private readonly DefaultSequenceStates _state = new();

    private ISequence _sequence;
    private bool _onTimerInput;


    private ISequenceBuilder GetOnTimerConfiguration()
    {
        return SequenceBuilder.Create()
            .AddForceState(_state.Off, () => !_onTimerInput)
            .AddTransition(_state.Off, _state.WaitOn, () => _onTimerInput, () => _sequence.Stopwatch.Restart())
            .AddTransition(_state.WaitOn, _state.On, () => _onTimerInput && _sequence.Stopwatch.ElapsedMilliseconds > 50)
            .SetInitialState(_state.Off)
            .DisableValidationForStates(_state.On);
    }

    [Theory]
    [InlineData(false, "Off")]
    [InlineData(true, "WaitOn")]
    public void Example_Usage_OnTimerConfiguration_Run_sync(bool timerInput, string expectedState)
    {
        _sequence = GetOnTimerConfiguration().Build();
        _onTimerInput = timerInput;

        _sequence.Run();

        var actual = _sequence.CurrentState;
        actual.Should().Be(expectedState);
    }

    [Theory]
    [InlineData(false, 0, "Off")]
    [InlineData(true, 0, "WaitOn")]
    [InlineData(true, 5, "WaitOn")]
    [InlineData(true, 51, "On")]
    public async Task Example_Usage_OnTimerConfiguration_Run_async(bool timerInput, int sleepTimeInMs, string expectedState)
    {
        _sequence = GetOnTimerConfiguration().Build();
        _onTimerInput = timerInput;


        await _sequence.RunAsync();

        await Task.Delay(sleepTimeInMs);

        await _sequence.RunAsync();


        var actual = _sequence.CurrentState;
        actual.Should().Be(expectedState);
    }
}
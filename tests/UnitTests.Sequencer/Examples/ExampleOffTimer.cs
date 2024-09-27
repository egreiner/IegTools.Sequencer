namespace UnitTests.Sequencer.Examples;

using IegTools.Sequencer;

public class ExampleOffTimer
{
    private readonly DefaultSequenceStates _state = new();

    private ISequence? _sequence;
    private bool _onTimerInput;


    private ISequenceBuilder GetOffTimerConfiguration()
    {
        return SequenceBuilder.Create()
            .AddForceState(_state.On, () => _onTimerInput)
            .AddTransition(_state.On, _state.Pending, () => _onTimerInput, () => _sequence?.Stopwatch.Restart())
            .AddTransition(_state.Pending, _state.Off, () => _onTimerInput && _sequence?.Stopwatch.ElapsedMilliseconds > 50)
            .SetInitialState(_state.Off)
            .DisableValidationForStates(_state.Off);
    }

    [Theory]
    [InlineData(false, "Off")]
    [InlineData(true, "On")]
    public void Example_Usage_OFFTimerConfiguration_Run_sync(bool timerInput, string expectedState)
    {
        _sequence = GetOffTimerConfiguration().Build();
        _onTimerInput = timerInput;

        _sequence.Run();

        var actual = _sequence.CurrentState;
        actual.Should().Be(expectedState);
    }

    [Theory(Skip="This test is not working as expected")]
    [InlineData(false, 0, "Off")]
    [InlineData(true, 0, "On")]
    [InlineData(true, 5, "On")]
    [InlineData(true, 51, "On")]
    public async Task Example_Usage_OFFTimerConfiguration_Run_async(bool timerInput, int sleepTimeInMs, string expectedState)
    {
        _sequence = GetOffTimerConfiguration().Build();
        _onTimerInput = timerInput;


        await _sequence.RunAsync();

        await Task.Delay(sleepTimeInMs);

        await _sequence.RunAsync();


        var actual = _sequence.CurrentState;
        actual.Should().Be(expectedState);
    }
}
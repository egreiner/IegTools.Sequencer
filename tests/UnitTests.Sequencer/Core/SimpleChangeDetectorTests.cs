namespace UnitTests.Sequencer.Core;

using IegTools.Sequencer.Core;


public class SimpleChangeDetectorTests
{
    private bool _eventValueChangedRaised;


    [Theory]
    [InlineData(null, null, false)]
    [InlineData(0, 0, false)]
    [InlineData(10, 10, false)]
    [InlineData(1, 2, true)]
    [InlineData(0.1, 0.1, false)]
    [InlineData(1.1, 2.1, true)]
    [InlineData("1", "1", false)]
    [InlineData("1", "2", true)]

    [InlineData(null, "2", true)]
    [InlineData(null, 2, true)]
    public void Expect_Event_raised_on_change_value<T>(T inputStart, T inputChange, bool expectRaised)
    {
        var sut = new SimpleChangeDetector<T>("UnitTest");
        T value = inputStart;
        sut.OnChange(() => value, () => { _eventValueChangedRaised = true; } );

        value = inputChange;
        sut.Detect();

        _eventValueChangedRaised.Should().Be(expectRaised);
    }


    [Theory]
    [InlineData(null, null)]
    [InlineData(0, 0)]
    [InlineData(10, 10)]
    [InlineData(1, 2)]
    [InlineData(0.1, 0.1)]
    [InlineData(1.1, 2.1)]
    [InlineData("1", "1")]
    [InlineData("1", "2")]

    [InlineData(null, "2")]
    [InlineData(null, 2)]
    public void Expect_disable_detection<T>(T inputStart, T inputChange)
    {
        var sut   = new SimpleChangeDetector<T>("UnitTest");
        T   value = inputStart;
        sut.OnChange(() => value, () => { _eventValueChangedRaised = true; } );

        value = inputChange;
        sut.Detect(false);

        _eventValueChangedRaised.Should().Be(false);
    }
}
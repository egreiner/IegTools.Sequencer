namespace UnitTests.Sequencer.StateAsString;

public class SequenceSetOnStateChangedActionTests
{
    private const string InitialState = "InitialState";


    [Theory]
    [InlineData(false, 0)]
    [InlineData(true, 100)]
    public void Test_SetOnStateChangedAction_run_sequence(bool constraint, int expected)
    {
        var x = 0;

        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(InitialState);
            builder.AddTransition(InitialState, "Stage1", () => constraint)
                .SetOnStateChangedAction(() => x = 100)
                .DisableValidation();
        });

        var sut = builder.Build();

        sut.Run();

        x.Should().Be(expected);
    }

    [Theory]
    [InlineData(false, true, 0)]
    [InlineData(true, true, 100)]
    [InlineData(false, false, 0)]
    [InlineData(true, false, 0)]
    public void Test_SetOnStateChangedAction_enabledFunc(bool constraint, bool enabled, int expected)
    {
        var x = 0;

        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(InitialState);
            builder.AddTransition(InitialState, "Stage1", () => constraint)
                .SetOnStateChangedAction(() => x = 100, () => enabled)
                .DisableValidation();
        });

        var sut = builder.Build();

        sut.Run();

        x.Should().Be(expected);
    }

    [Theory]
    [InlineData(InitialState, 0)]
    [InlineData("Stage1", 100)]
    public void Test_SetOnStateChangedAction_set_state(string setState, int expected)
    {
        var x = 0;

        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(InitialState);
            builder.AddTransition(InitialState, "Stage1", () => false)
                .SetOnStateChangedAction(() => x = 100)
                .DisableValidation();
        });

        var sut = builder.Build();

        // sut.Run();
        sut.SetState(setState);

        x.Should().Be(expected);
    }

    [Theory]
    [InlineData("Stage1")]
    public void Test_SetOnStateChangedAction_can_should_throw_exceptions(string setState)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(InitialState);
            builder.AddTransition(InitialState, "Stage1", () => false)
                .SetOnStateChangedAction(() => throw new Exception("Test"))
                .DisableValidation();
        });
        var sut = builder.Build();

        Action act = () => sut.SetState(setState);

        act.Should().Throw<Exception>();
    }
}
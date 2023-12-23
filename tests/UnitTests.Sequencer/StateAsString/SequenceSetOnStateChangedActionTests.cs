﻿namespace UnitTests.Sequencer.StateAsString;

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
    [InlineData(InitialState)]
    [InlineData("Stage1")]
    public void Test_SetOnStateChangedAction_can_handle_exceptions(string setState)
    {
        var x = 0;

        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(InitialState);
            builder.AddTransition(InitialState, "Stage1", () => false)
                .SetOnStateChangedAction(() => throw new Exception("Test"))
                .DisableValidation();
        });

        var sut = builder.Build();

        // sut.Run();
        sut.SetState(setState).Should();
    }
}
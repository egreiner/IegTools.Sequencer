namespace UnitTests.Sequencer.StateAsString;

public class SequenceSetStateTests
{
    private const string InitialState = "InitialState";


    [Theory]
    [InlineData(true, "Set")]
    [InlineData(false, InitialState)]
    public void Test_SetState(bool constraint, string expected)
    {
        var builder = SequenceBuilder.Configure(builder =>
            builder.SetInitialState(InitialState)
                .AddForceState("Force", () => constraint)
                .AddTransition("Set", InitialState, () => constraint)
                .DisableValidation());

        var sut = builder.Build();

        sut.SetState("Set", () => constraint);

        // no Execute is necessary

        sut.CurrentState.Should().Be(expected);
    }


    [Theory]
    [InlineData(true, InitialState, "Set", "Set")]
    [InlineData(false, InitialState, "Set", InitialState)]
    [InlineData(true, "Set", "Reset", "Set")]
    [InlineData(false, "Set", "Reset", "Set")]
    [InlineData(true, InitialState, InitialState, InitialState)]
    [InlineData(false, InitialState, InitialState, InitialState)]
    public void Test_try_to_SetState_to_not_registered_state(bool constraint, string currentState, string setState, string expected)
    {
        var builder = SequenceBuilder.Configure(builder =>
            builder.SetInitialState(InitialState)
                .AddForceState("Force", () => constraint)
                .AddTransition("Set", InitialState, () => constraint)
                .DisableValidation());

        var sut = builder.Build();

        sut.SetState(currentState);
        sut.SetState(setState, () => constraint);

        // no Execute is necessary

        sut.CurrentState.Should().Be(expected);
    }

    [Theory]
    [InlineData(true, "Set")]
    [InlineData(false, InitialState)]
    public void Test_SetState_Only_Last_Counts(bool constraint, string expected)
    {
        var builder = SequenceBuilder.Configure(builder =>
            builder.SetInitialState(InitialState)
                .AddForceState("Force", () => constraint)
                .AddTransition("Set", InitialState, () => constraint)
                .AddTransition(InitialState, "test", () => constraint)
                .DisableValidation());

        var sut = builder.Build();

        sut.SetState("test", () => constraint);
        sut.SetState("Set", () => constraint);

        // no Execute is necessary

        sut.CurrentState.Should().Be(expected);
    }
}
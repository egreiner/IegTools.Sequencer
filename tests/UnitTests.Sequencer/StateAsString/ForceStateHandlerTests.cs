namespace UnitTests.Sequencer.StateAsString;

public class ForceStateHandlerTests
{
    private const string InitialState = "InitialState";


    [Theory]
    [InlineData(true, "Force")]
    [InlineData(false, InitialState)]
    public void Test_ForceState(bool constraint, string expected)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(InitialState);
            builder.AddForceState("Force", () => constraint)
                .DisableValidation();
        });

        var sut = builder.Build().Run();

        sut.CurrentState.Should().Be(expected);
    }

    [Theory]
    [InlineData(true, "Force1")]
    [InlineData(false, "Force2")]
    public void Test_AllForceStates_are_working(bool constraint, string expected)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(InitialState)
                .AddForceState("Force1", () => constraint)
                .AddForceState("Force2", () => !constraint)
                .DisableValidation();
        });

        var sut = builder.Build().Run();

        sut.CurrentState.Should().Be(expected);
    }

    [Theory]
    [InlineData(true, "Set")]
    [InlineData(false, InitialState)]
    public void Test_Set(bool constraint, string expected)
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

    [Theory]
    [InlineData("Force", true)]
    [InlineData("NotDefined", false)]
    public void Test_IsRegisteredState(string state, bool expected)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(InitialState);
            builder.AddForceState("Force", () => true)
                .DisableValidation();
        });


        var sut = builder.Build();

        sut.IsRegisteredState(state).Should().Be(expected);
    }
}
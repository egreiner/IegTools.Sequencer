using IegTools.Sequencer.Extensions;

namespace UnitTests.Sequencer.StateAsString;

public class ForceStateDescriptorTests
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

        var actual = sut.CurrentState;
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(true, "Force1")]
    [InlineData(false, "Force2")]
    public void Test_AllForceStatuses_are_working(bool constraint, string expected)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(InitialState)
                .AddForceState("Force1", () => constraint)
                .AddForceState("Force2", () => !constraint)
                .DisableValidation();
        });

        var sut = builder.Build().Run();

        var actual = sut.CurrentState;
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(true, "Set")]
    [InlineData(false, InitialState)]
    public void Test_Set(bool constraint, string expected)
    {
        var builder = SequenceBuilder.Configure(builder =>
            builder.SetInitialState(InitialState)
                .AddForceState("Force", () => constraint)
                .DisableValidation());

        var sut = builder.Build();

        sut.SetState("Set", () => constraint);

        // no Execute is necessary

        var actual = sut.CurrentState;
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(true, "Set")]
    [InlineData(false, InitialState)]
    public void Test_SetState_Only_Last_Counts(bool constraint, string expected)
    {
        var builder = SequenceBuilder.Configure(builder =>
            builder.SetInitialState(InitialState)
                .AddForceState("Force", () => constraint)
                .DisableValidation());

        var sut = builder.Build();

        sut.SetState("test", () => constraint);
        sut.SetState("Set", () => constraint);

        // no Execute is necessary

        var actual = sut.CurrentState;
        Assert.Equal(expected, actual);
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

        var actual = sut.IsRegisteredState(state);
        Assert.Equal(expected, actual);
    }
}
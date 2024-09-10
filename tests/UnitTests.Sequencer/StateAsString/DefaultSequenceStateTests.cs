namespace UnitTests.Sequencer.StateAsString;

public class DefaultSequenceStateTests
{
    private readonly DefaultSequenceStates _states = new();

    [Fact]
    public void Test_Constrain_Add_Conditional_State()
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(_states.Initializing);
            builder.AddTransition(_states.Initializing, _states.Initialized, () => true)
                .DisableValidation();
        });

        var sut = builder.Build();

        sut.Run();

        sut.CurrentState.Should().Be(_states.Initialized);
    }
}
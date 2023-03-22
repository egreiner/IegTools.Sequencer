namespace UnitTests.Sequencer.StateAsEnum;

using IegTools.Sequencer;

public class AnyStateTransitionDescriptorEnumTests
{
    [Theory]
    [InlineData(TestEnum.State1, TestEnum.StateX, true, TestEnum.State1, TestEnum.State2)]
    [InlineData(TestEnum.State1, TestEnum.State1, false, TestEnum.State1, TestEnum.State2)]

    [InlineData(TestEnum.State2, TestEnum.StateX, true, TestEnum.State1, TestEnum.State2)]
    [InlineData(TestEnum.State2, TestEnum.State2, false, TestEnum.State1, TestEnum.State2)]

    [InlineData(TestEnum.State3, TestEnum.State3, true, TestEnum.State1, TestEnum.State2)]
    [InlineData(TestEnum.State3, TestEnum.State3, false, TestEnum.State1, TestEnum.State2)]
    public void Test_AddAnyTransition(TestEnum currentState, TestEnum expected, bool constraint, params TestEnum[] currentStateContains)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(TestEnum.State1);
            
            builder.AddTransition(TestEnum.State1, TestEnum.State2, () => false);
            builder.AddTransition(TestEnum.State2, TestEnum.State3, () => false);
            
            builder.AddAnyTransition(currentStateContains, TestEnum.StateX, () => constraint);

            builder.DisableValidation();
        });

        var sut = builder.Build();

        sut.SetState(currentState);
        sut.Run();

        sut.CurrentState.Should().Be(expected.ToString());
    }
}
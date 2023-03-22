namespace UnitTests.Sequencer.StateAsEnum;

using IegTools.Sequencer.Images;

public class ContainsStateTransitionDescriptorEnumTests
{
    [Theory]
    [InlineData(TestEnum.State1, true, TestEnum.StateX)]
    [InlineData(TestEnum.State1, false, TestEnum.State1)]
    [InlineData(TestEnum.State2, true, TestEnum.StateX)]
    [InlineData(TestEnum.State2, false, TestEnum.State2)]
    [InlineData(TestEnum.State3, true, TestEnum.StateX)]
    [InlineData(TestEnum.State3, false, TestEnum.State3)]
    [InlineData(TestEnum.StateX, true, TestEnum.StateX)]
    [InlineData(TestEnum.StateX, false, TestEnum.StateX)]
    public void Test_AddContainsTransition_one_compareState(TestEnum currentState, bool constraint, TestEnum expected)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(TestEnum.State1);
            
            builder.AddTransition(TestEnum.State1, TestEnum.State2, () => false);
            builder.AddTransition(TestEnum.State2, TestEnum.State3, () => false);
            
            builder.AddContainsTransition("State", TestEnum.StateX, () => constraint);

            builder.DisableValidation();
        });

        var sut = builder.Build();

        sut.SetState(currentState);
        sut.Run();

        sut.CurrentState.Should().Be(expected.ToString());
    }
}
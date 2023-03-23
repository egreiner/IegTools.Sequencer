namespace UnitTests.Sequencer.StateAsEnum;

using IegTools.Sequencer;

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
    
    [InlineData(TestEnum.Force, true, TestEnum.Force)]
    [InlineData(TestEnum.Force, false, TestEnum.Force)]
    public void Test_AddContainsTransition_one_compareState(TestEnum currentState, bool constraint, TestEnum expected)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(TestEnum.State1);
            builder.AddForceState(TestEnum.Force, () => false);
            
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

    [Theory]
    [InlineData(TestEnum.State1, true, 1)]
    [InlineData(TestEnum.State2, true, 1)]
    [InlineData(TestEnum.State3, true, 1)]
    [InlineData(TestEnum.StateX, true, 0)]

    [InlineData(TestEnum.State1, false, 0)]
    [InlineData(TestEnum.State2, false, 0)]
    [InlineData(TestEnum.State3, false, 0)]
    [InlineData(TestEnum.StateX, false, 0)]
    
    [InlineData(TestEnum.Force, true, 0)]
    [InlineData(TestEnum.Force, false, 0)]
    public void Test_AddContainsTransition_execute_just_once(TestEnum currentState, bool constraint, int expected)
    {
        var actual = 0;
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(TestEnum.State1);
            builder.AddForceState(TestEnum.Force, () => false);

            builder.AddTransition(TestEnum.State1, TestEnum.State2, () => false);
            builder.AddTransition(TestEnum.State2, TestEnum.State3, () => false);

            builder.AddContainsTransition("State", TestEnum.StateX, () => constraint, () => actual++);

            builder.DisableValidation();
        });

        var sut = builder.Build();

        sut.SetState(currentState);

        for (var index = 0; index < 3; index++)
            sut.Run();

        actual.Should().Be(expected);
    }
}
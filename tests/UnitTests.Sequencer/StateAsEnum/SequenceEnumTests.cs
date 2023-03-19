using IegTools.Sequencer.Extensions;

namespace UnitTests.Sequencer.StateAsEnum;

public class SequenceEnumTests
{
    private const TestEnum InitialState = TestEnum.InitialState;


    [Theory]
    [InlineData(true, TestEnum.Force)]
    [InlineData(false, InitialState)]
    public void Test_ForceState(bool constraint, TestEnum expected)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(InitialState);
            builder.AddForceState(TestEnum.Force, () => constraint)
                   .DisableValidation();
        });

        var sut = builder.Build().Run();

        var actual = sut.CurrentState;
        Assert.Equal(expected.ToString(), actual);
    }

    [Theory]
    [InlineData(true, TestEnum.State1)]
    [InlineData(false, TestEnum.State2)]
    public void Test_AllForceStatuses_are_working(bool constraint, TestEnum expected)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(InitialState);
            builder.AddForceState(TestEnum.State1, () => constraint)
                   .AddForceState(TestEnum.State2, () => !constraint)
                   .DisableValidation();
        });

        var sut = builder.Build().Run();

        var actual = sut.CurrentState;
        Assert.Equal(expected.ToString(), actual);
    }

    [Theory]
    [InlineData(true, TestEnum.State1)]
    [InlineData(false, InitialState)]
    public void Test_Set(bool constraint, TestEnum expected)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(InitialState);
            builder.AddForceState(TestEnum.Force, () => constraint)
                   .DisableValidation();
        });

        var sut = builder.Build();

        sut.SetState(TestEnum.State1, () => constraint);

        // no Execute is necessary

        var actual = sut.CurrentState;
        Assert.Equal(expected.ToString(), actual);
    }

    [Theory]
    [InlineData(true, TestEnum.State1)]
    [InlineData(false, InitialState)]
    public void Test_SetState_Only_Last_Counts(bool constraint, TestEnum expected)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(InitialState);
            builder.AddForceState(TestEnum.Force, () => constraint)
                   .DisableValidation();

        });

        var sut = builder.Build();

        sut.SetState(TestEnum.StateX, () => constraint);
        sut.SetState(TestEnum.State1, () => constraint);

        // no Execute is necessary

        var actual = sut.CurrentState;
        Assert.Equal(expected.ToString(), actual);
    }

    [Theory]
    [InlineData(TestEnum.State1, true, TestEnum.State2)]
    [InlineData(TestEnum.State1, false, TestEnum.State1)]
    [InlineData(TestEnum.StateX, true, TestEnum.InitialState)]
    [InlineData(TestEnum.StateX, false, TestEnum.InitialState)]
    public void Test_Constrain_Add_Conditional_State(TestEnum currentState, bool constraint, TestEnum expected)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(InitialState);
            builder.AddTransition(TestEnum.State1, TestEnum.State2, () => constraint)
                   .DisableValidation();
        });

        var sut = builder.Build();

        sut.SetState(currentState);
        sut.Run();

        var actual = sut.CurrentState;
        Assert.Equal(expected.ToString(), actual);
    }


    [Theory]
    [InlineData(TestEnum.State1, TestEnum.State2, false)]
    [InlineData(TestEnum.State1, TestEnum.State1, true)]
    public void Test_HasCurrentState(TestEnum currentState, TestEnum queryState, bool expected)
    {
        var builder = SequenceBuilder.Configure(builder =>
            builder.AddTransition(TestEnum.State1, TestEnum.State2, () => false)
                .DisableValidation());

        var sut = builder.Build();

        sut.SetState(currentState);
        sut.Run();

        var actual = sut.HasCurrentState(queryState);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(false, TestEnum.State1, TestEnum.State2)]
    [InlineData(false, TestEnum.State1, TestEnum.State2, TestEnum.InitialState, TestEnum.Force)]
    [InlineData(true, TestEnum.State1, TestEnum.State1, TestEnum.InitialState, TestEnum.Force)]
    public void Test_HasAnyCurrentState(bool expected, TestEnum currentState, params TestEnum[] queryStates)
    {
        var builder = SequenceBuilder.Configure(builder =>
            builder.AddTransition(TestEnum.State1, TestEnum.State2, () => false)
                .DisableValidation());

        var sut = builder.Build();

        sut.SetState(currentState);
        sut.Run();

        var actual = sut.HasAnyCurrentState(queryStates);
        Assert.Equal(expected, actual);
    }


    [Theory]
    [InlineData(TestEnum.State1, true)]
    [InlineData(TestEnum.State2, true)]
    [InlineData(TestEnum.StateX, false)]
    public void Test_IsRegisteredState(TestEnum queryState, bool expected)
    {
        var builder = SequenceBuilder.Configure(builder =>
            builder.AddTransition(TestEnum.State1, TestEnum.State2, () => false)
                .DisableValidation());

        var sut = builder.Build();

        sut.Run();

        var actual = sut.IsRegisteredState(queryState);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(TestEnum.State1, true, 1)]
    [InlineData(TestEnum.State1, false, 0)]
    [InlineData(TestEnum.StateX, true, 0)]
    [InlineData(TestEnum.StateX, false, 0)]
    public void Test_Action_Add_Conditional_State(TestEnum currentState, bool constraint, int expected)
    {
        var countStarts = 0;
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(InitialState);
            builder.AddTransition(TestEnum.State1, TestEnum.State2, () => constraint, () => countStarts = 1)
                   .DisableValidation();
        });

        var sut = builder.Build();

        sut.SetState(currentState);
        sut.Run();

        var actual = countStarts;
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(TestEnum.State1, true, 2)]
    public void Test_Concatenation_Add_Conditional_State(TestEnum currentState, bool constraint, int expected)
    {
        var countStarts = 0;
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState(InitialState);
            builder.AddTransition(TestEnum.State1, TestEnum.State2, () => constraint, () => countStarts++);
            builder.AddTransition(TestEnum.State2, TestEnum.StateX, () => constraint, () => countStarts++)
                   .DisableValidation();
        });

        var sut = builder.Build();

        sut.SetState(currentState);
        sut.Run();

        var actualCount = countStarts;
        Assert.Equal(expected, actualCount);

        var actualState = sut.CurrentState;
        Assert.Equal(TestEnum.StateX.ToString(), actualState);
    }


    [Theory]
    [InlineData(TestEnum.State1, TestEnum.State2, 0)]
    [InlineData(TestEnum.State1, TestEnum.State1, 1)]
    public void Test_AddStateActionDescriptor(TestEnum state, TestEnum currentState, int expected)
    {
        var result = 0;
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.AddStateAction(state, () => result++)
                .DisableValidation();
        });

        var sut = builder.Build();

        sut.SetState(currentState);
        sut.Run();

        var actual = result;
        Assert.Equal(expected, actual);
    }
}
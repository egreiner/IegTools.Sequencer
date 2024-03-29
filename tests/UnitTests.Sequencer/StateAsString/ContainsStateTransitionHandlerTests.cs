﻿namespace UnitTests.Sequencer.StateAsString;

using IegTools.Sequencer;

public class ContainsStateTransitionHandlerTests
{
    [Theory]
    [InlineData("State1", true, "StateX")]
    [InlineData("State1", false, "State1")]
    
    [InlineData("State2", true, "StateX")]
    [InlineData("State2", false, "State2")]
    
    [InlineData("State3", true, "StateX")]
    [InlineData("State3", false, "State3")]
    
    [InlineData("StateX", true, "StateX")]
    [InlineData("StateX", false, "StateX")]
    
    [InlineData("Force", true, "Force")]
    [InlineData("Force", false, "Force")]
    public void Test_AddContainsTransition_one_compareState(string currentState, bool constraint, string expected)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState("State1");
            builder.AddForceState("Force", () => false);

            builder.AddTransition("State1", "State2", () => false);
            builder.AddTransition("State2", "State3", () => false);
            
            builder.AddContainsTransition("State", "StateX", () => constraint);

            builder.DisableValidation();
        });

        var sut = builder.Build();

        sut.SetState(currentState);
        sut.Run();

        sut.CurrentState.Should().Be(expected);
    }

    
    [Theory]
    [InlineData("State1", true, 1)]
    [InlineData("State2", true, 1)]
    [InlineData("State3", true, 1)]
    [InlineData("StateX", true, 0)]

    [InlineData("State1", false, 0)]
    [InlineData("State2", false, 0)]
    [InlineData("State3", false, 0)]
    [InlineData("StateX", false, 0)]

    [InlineData("Force", true, 0)]
    [InlineData("Force", false, 0)]
    public void Test_AddContainsTransition_execute_just_once(string currentState, bool constraint, int expected)
    {
        var actual = 0;
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState("State1");
            builder.AddForceState("Force", () => false);

            builder.AddTransition("State1", "State2", () => false);
            builder.AddTransition("State2", "State3", () => false);

            builder.AddContainsTransition("State", "StateX", () => constraint, () => actual++);

            builder.DisableValidation();
        });

        var sut = builder.Build();

        sut.SetState(currentState);

        for (var index = 0; index < 3; index++)
            sut.Run();

        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData("State1", true)]
    [InlineData("State2", true)]
    [InlineData("State3", true)]
    [InlineData("StateX", true)]
    public void Test_Handler_can_handle_exceptions(string currentState, bool constraint)
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState("State1");
            builder.AddForceState("Force", () => false);

            builder.AddTransition("State1", "State2", () => false);
            builder.AddTransition("State2", "State3", () => false);

            builder.AddContainsTransition("State", "StateX", () => constraint, () => throw new Exception("Test"));

            builder.DisableValidation();
        });

        var sut = builder.Build();

        sut.SetState(currentState);

        for (var index = 0; index < 3; index++)
            sut.Run();
    }
}
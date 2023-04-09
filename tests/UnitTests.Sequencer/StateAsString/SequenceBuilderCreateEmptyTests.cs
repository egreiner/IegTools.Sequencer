namespace UnitTests.Sequencer.StateAsString;

public class SequenceBuilderCreateEmptyTests
{
    [Fact]
    public void Test_CreateEmpty_does_not_throw_ValidationException()
    {
        var builder = SequenceBuilder.CreateEmpty();

        var build = () => builder.Build();
        build.Should().NotThrow();
    }

    [Fact]
    public void Test_Sequence_Run_does_not_throw_ValidationException()
    {
        var builder = SequenceBuilder.CreateEmpty();

        var sequence = builder.Build();

        var run = () => sequence.Run();
        run.Should().NotThrow();
    }

    [Fact]
    public void Test_CreateEmpty_default_State()
    {
        var builder = SequenceBuilder.CreateEmpty();

        var sequence = builder.Build();
        sequence.Run();

        sequence.CurrentState.Should().Be("Empty");
    }

    [Fact]
    public void Test_CreateEmpty_custom_State()
    {
        var builder = SequenceBuilder.CreateEmpty("CustomState");

        var sequence = builder.Build();
        sequence.Run();

        sequence.CurrentState.Should().Be("CustomState");
    }
}
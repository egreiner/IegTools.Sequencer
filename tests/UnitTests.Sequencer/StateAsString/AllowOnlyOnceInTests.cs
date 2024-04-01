namespace UnitTests.Sequencer.StateAsString;

public class AllowOnlyOnceInTests
{
    private const string InitialState = "InitialState";


    [Fact]
    public void Test_AllowOnlyOnceIn_not_set()
    {
        var x = 0;
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState("State1");
            builder.AddTransition("State1", "State2", () => true, () => x++)
                .DisableValidation();
        });

        var sut = builder.Build();

        sut.SetState("State1");

        for (int i = 0; i < 3; i++)
        {
            sut.Run();
            sut.SetState("State1");
        }

        x.Should().Be(3);
    }

    [Fact]
    public void Test_AllowOnlyOnceIn_set()
    {
        var x = 0;
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState("State1");
            builder.AddTransition("State1", "State2", () => true, () => x++)
                .AllowOnlyOnceIn(TimeSpan.FromSeconds(1))
                .DisableValidation();
        });

        var sut = builder.Build();

        sut.SetState("State1");

        for (int i = 0; i < 3; i++)
        {
            sut.Run();
            sut.SetState("State1");
        }

        x.Should().Be(1);
    }

    [Fact]
    public void Test_AllowOnlyOnceIn_set2()
    {
        var x = 0;
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState("State1");
            builder.AddTransition("State1", "State2", () => true, () => x++)
                .AllowOnlyOnceIn(TimeSpan.FromMilliseconds(1))
                .DisableValidation();
        });

        var sut = builder.Build();

        sut.SetState("State1");

        for (int i = 0; i < 3; i++)
        {
            sut.Run();
            sut.SetState("State1");
            Thread.Sleep(1);
        }

        x.Should().Be(3);
    }

    [Theory]
    [InlineData("State1", 1, 1)]
    [InlineData("State2", 1, 3)]
    public void Test_AllowOnlyOnceIn_set_two_Handler(string setState, int expectedX,int expectedY)
    {
        var x = 0;
        var y = 0;
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState("State1");
            builder
                .AddTransition("State1", "State2", () => true, () => x++)
                .AllowOnlyOnceIn(TimeSpan.FromSeconds(1))
                .AddTransition("State2", "State3", () => true, () => y++)
                .AllowOnlyOnceIn(TimeSpan.FromMilliseconds(1))
                .DisableValidation();
        });

        var sut = builder.Build();

        sut.SetState("State1");

        for (int i = 0; i < 3; i++)
        {
            sut.Run();
            sut.SetState(setState);
            Thread.Sleep(1);
        }

        x.Should().Be(expectedX);
        y.Should().Be(expectedY);
    }

    [Theory]
    [InlineData("State1", 3, 1)]
    [InlineData("State2", 1, 1)]
    public void Test_AllowOnlyOnceIn_set_two_Handler2(string setState, int expectedX,int expectedY)
    {
        var x = 0;
        var y = 0;
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState("State1");
            builder
                .AddTransition("State1", "State2", () => true, () => x++)
                .AllowOnlyOnceIn(TimeSpan.FromMilliseconds(1))
                .AddTransition("State2", "State3", () => true, () => y++)
                .AllowOnlyOnceIn(TimeSpan.FromSeconds(1))
                .DisableValidation();
        });

        var sut = builder.Build();

        sut.SetState("State1");

        for (int i = 0; i < 3; i++)
        {
            sut.Run();
            sut.SetState(setState);
            Thread.Sleep(1);
        }

        x.Should().Be(expectedX);
        y.Should().Be(expectedY);
    }
}
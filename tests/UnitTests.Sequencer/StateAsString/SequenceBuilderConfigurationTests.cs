namespace UnitTests.Sequencer.StateAsString;

using IegTools.Sequencer.Validation;

public class SequenceBuilderConfigurationTests
{
    [Fact]
    public void Test_Build_with_default_Validator()
    {
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState("State1")
                .DisableValidation();
        });

        var build = () => builder.Build();
        build.Should().NotThrow();
    }

    [Fact]
    public void Test_Build_with_custom_Validator()
    {
        var builder = SequenceBuilder.Configure(new SequenceConfigurationValidator(), builder =>
        {
            builder.SetInitialState("State1")
                .DisableValidation();
        });

        var build = () => builder.Build();
        build.Should().NotThrow();
    }
}
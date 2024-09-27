namespace UnitTests.Sequencer.Extensions;

public class StringExtensionsTests
{
    [Fact]
    public void MatchesAny_ShouldBeTrue_WhenStringParamsContainsText()
    {
        var text = "text";

        var actual  = text.MatchesAny("text", "text1", "text2");

        actual.Should().BeTrue();
    }

    [Fact]
    public void MatchesAny_ShouldBeFalse_WhenStringParamsDoesNotContainText()
    {
        var text = "not found";

        var actual  = text.MatchesAny("text", "text1", "text2");

        actual.Should().BeFalse();
    }

    [Fact]
    public void MatchesAny_ShouldBeTrue_WhenStringArrayContainsText()
    {
        var text = "text";
        var stringArray = new[] { "text", "text1", "text2" };

        var actual  = text.MatchesAny(stringArray);

        actual.Should().BeTrue();
    }

    [Fact]
    public void MatchesAny_ShouldBeFalse_WhenStringArrayDoesNotContainText()
    {
        var text = "not found";
        var stringArray = new[] { "text", "text1", "text2" };

        var actual  = text.MatchesAny(stringArray);

        actual.Should().BeFalse();
    }

    [Fact]
    public void MatchesAny_ShouldBeFalse_WhenStringArrayOrStringParamsContainsText()
    {
        var text = "text3";
        var stringArray = new[] { "text", "text1", "text2" };

        var actual  = text.MatchesAny(stringArray, "text3", "text4");

        actual.Should().BeTrue();
    }

    [Fact]
    public void MatchesAny_ShouldBeFalse_WhenStringArrayAndStringParamsDoNotContainText()
    {
        var text = "not found";
        var stringArray = new[] { "text", "text1", "text2" };

        var actual  = text.MatchesAny(stringArray, "text3", "text4");

        actual.Should().BeFalse();
    }
}
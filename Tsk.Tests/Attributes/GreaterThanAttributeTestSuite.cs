using Tsk.HttpApi.Validation;

namespace Tsk.Tests.Attributes;

public class GreaterThanAttributeTestSuite
{
    [Fact]
    public void GreaterThantAttribute_WhenValueIsGreater_ShouldSucceed()
    {
        var attribute = new GreaterThanAttribute(0.5);
        var result = attribute.IsValid(0.51);
        result.Should().BeTrue();
    }

    [Fact]
    public void GreaterThantAttribute_WhenValueEqual_ShouldFail()
    {
        var attribute = new GreaterThanAttribute(0.5);
        var result = attribute.IsValid(0.5);
        result.Should().BeFalse();
    }

    [Fact]
    public void GreaterThantAttribute_WhenValueIsLess_ShouldFail()
    {
        var attribute = new GreaterThanAttribute(0.5);
        var result = attribute.IsValid(0.49);
        result.Should().BeFalse();
    }
}

using Tsk.HttpApi.Validation;

namespace Tsk.Tests.Attributes;

public class PriceAttributeTestSuite
{
    [Fact]
    public void PriceAttribute_WhenValueIsValid_ShouldSucceed()
    {
        var attribute = new PriceAttribute();
        var result = attribute.IsValid(9.99m);
        result.Should().BeTrue();
    }

    [Fact]
    public void PriceAttribute_WhenValueHasMoreThan2FractionalDigits_ShouldFail()
    {
        var attribute = new PriceAttribute();
        var result = attribute.IsValid(9.999m);
        result.Should().BeFalse();
    }

    [Fact]
    public void PriceAttribute_WhenValueHasLessThan2FractionalDigits_ShouldFail()
    {
        var attribute = new PriceAttribute();
        var result = attribute.IsValid(9.9m);
        result.Should().BeFalse();
    }

    [Fact]
    public void PriceAttribute_WhenValueIsNotDecimal_ShouldFail()
    {
        var attribute = new PriceAttribute();
        var result = attribute.IsValid(9.99);
        result.Should().BeFalse();
    }

    [Fact]
    public void PriceAttribute_WhenValueIsZero_ShouldFail()
    {
        var attribute = new PriceAttribute();
        var result = attribute.IsValid(0m);
        result.Should().BeFalse();
    }

    [Fact]
    public void PriceAttribute_WhenValueIsLessThanZero_ShouldFail()
    {
        var attribute = new PriceAttribute();
        var result = attribute.IsValid(-9.99m);
        result.Should().BeFalse();
    }
}

using System.Diagnostics.CodeAnalysis;
using Tsk.HttpApi.Validation;

namespace Tsk.Tests.UnitTests.Attributes;

[SuppressMessage("ReSharper", "UseCollectionExpression")]
public class PriceAttributeTestSuite
{
    [Theory]
    [MemberData(nameof(ValidPrices))]
    public void PriceAttribute_WhenValueIsValid_ShouldSucceed(decimal value)
    {
        var attribute = new PriceAttribute();
        attribute.IsValid(value).Should().BeTrue();
    }

    public static TheoryData<decimal> ValidPrices =
        new() { 0.09m, 0.9m, 0.99m, 9.99m };

    [Theory]
    [MemberData(nameof(PricesWithMoreThan2DecimalPlaces))]
    public void PriceAttribute_WhenValueHasMoreThan2DecimalPlaces_ShouldFail(decimal value)
    {
        var attribute = new PriceAttribute();
        attribute.IsValid(value).Should().BeFalse();
    }

    public static TheoryData<decimal> PricesWithMoreThan2DecimalPlaces()
    {
        return new() { 9.999m, 9.9999m, 9.99999m, 9.999999m, 9.9999999m };
    }

    [Theory]
    [MemberData(nameof(NegativePrices))]
    public void PriceAttribute_WhenValueIsNegative_ShouldFail(decimal value)
    {
        var attribute = new PriceAttribute();
        attribute.IsValid(value).Should().BeFalse();
    }

    public static TheoryData<decimal> NegativePrices()
    {
        return new() { -9.99m, -0.99m, -0.9m, -0.09m };
    }

    [Fact]
    public void PriceAttribute_WhenValueIsZero_ShouldFail()
    {
        var attribute = new PriceAttribute();
        attribute.IsValid(0m).Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(PricesOfOtherTypes))]
    public void PriceAttribute_WhenValueIsNotDecimal_ShouldFail(object value)
    {
        var attribute = new PriceAttribute();
        attribute.Invoking(x => x.IsValid(value)).Should().Throw<Exception>();
    }

    public static TheoryData<object> PricesOfOtherTypes()
    {
        return new() { 9, 9.99f, 9.99d, "9.99" };
    }
}

using Bogus;
using Tsk.HttpApi.Products;

namespace Tsk.Tests.Products;

public static class ProductTestData
{
    private static readonly Faker faker = new();
    private static readonly PriceRange defaultPriceRange = new()
    {
        Min = 0.01,
        Max = 100.00
    };

    public static ProductEntity GenerateProduct() =>
        GenerateProduct(defaultPriceRange);

    public static ProductEntity GenerateProduct(PriceRange priceRange) =>
        new ProductEntity
        {
            Id = GenerateProductId(),
            Title = GenerateProductTitle(),
            Price = GenerateProductPrice(priceRange)
        };

    public static IReadOnlyCollection<ProductEntity> GenerateProducts(int count) =>
        GenerateProducts(count, defaultPriceRange);

    public static IReadOnlyCollection<ProductEntity> GenerateProducts(int count, PriceRange priceRange) =>
        Enumerable
            .Range(0, count)
            .Select(_ => GenerateProduct(priceRange))
            .ToList();

    public static CreateProductDto GenerateCreateProductDto() =>
        new CreateProductDto
        {
            Title = GenerateProductTitle(),
            Price = GenerateProductPrice(defaultPriceRange)
        };

    public static UpdateProductDto GenerateUpdateProductDto() =>
        new UpdateProductDto
        {
            Title = GenerateProductTitle(),
            Price = GenerateProductPrice(defaultPriceRange)
        };

    private static Guid GenerateProductId() =>
        Guid.NewGuid();

    private static string GenerateProductTitle() =>
        faker.Commerce.ProductName();

    private static double GenerateProductPrice(PriceRange priceRange)
    {
        var minPriceInCents = (int)(priceRange.Min * 100);
        var maxPriceInCents = (int)(priceRange.Max * 100);
        var priceInCents = Random.Shared.Next(minPriceInCents, maxPriceInCents);
        return priceInCents / 100d;
    }
}

public class PriceRange
{
    public double Min { get; init; }
    public double Max { get; set; }
}

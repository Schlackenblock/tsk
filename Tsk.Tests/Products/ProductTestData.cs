using Bogus;
using Tsk.HttpApi.Products;

namespace Tsk.Tests.Products;

public static class ProductTestData
{
    private static readonly Faker faker = new();

    public static ProductEntity GenerateProduct() =>
        GenerateProduct(PriceRange.Default);

    private static ProductEntity GenerateProduct(PriceRange priceRange) =>
        new ProductEntity
        {
            Id = GenerateProductId(),
            Title = GenerateProductTitle(),
            Price = GenerateProductPrice(priceRange)
        };

    public static IReadOnlyCollection<ProductEntity> GenerateProducts(int count) =>
        GenerateProducts(count, PriceRange.Default);

    public static IReadOnlyCollection<ProductEntity> GenerateProducts(int count, PriceRange priceRange) =>
        Enumerable
            .Range(0, count)
            .Select(_ => GenerateProduct(priceRange))
            .ToList();

    public static CreateProductDto GenerateCreateProductDto() =>
        new CreateProductDto
        {
            Title = GenerateProductTitle(),
            Price = GenerateProductPrice(PriceRange.Default)
        };

    public static UpdateProductDto GenerateUpdateProductDto() =>
        new UpdateProductDto
        {
            Title = GenerateProductTitle(),
            Price = GenerateProductPrice(PriceRange.Default)
        };

    private static Guid GenerateProductId() =>
        Guid.NewGuid();

    private static string GenerateProductTitle() =>
        faker.Commerce.ProductName();

    private static double GenerateProductPrice(PriceRange priceRange)
    {
        var floorInCents = (int)(priceRange.Min * 100);
        var ceilingInCents = (int)(priceRange.Max * 100);
        var priceInCents = Random.Shared.Next(floorInCents, ceilingInCents);
        return priceInCents / 100d;
    }
}

public class PriceRange
{
    public static PriceRange Default =>
        new();

    public double Min { get; init; } = 0.01;
    public double Max { get; init; } = 10_000.00;
}

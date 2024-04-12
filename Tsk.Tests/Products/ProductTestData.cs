using Bogus;
using Tsk.HttpApi.Products;

namespace Tsk.Tests.Products;

public static class ProductTestData
{
    private static readonly Faker faker = new();

    public static ProductEntity GenerateProduct() =>
        new ProductEntity
        {
            Id = GenerateProductId(),
            Title = GenerateProductTitle(),
            Price = GenerateProductPrice()
        };

    public static IReadOnlyCollection<ProductEntity> GenerateProducts(int count) =>
        Enumerable
            .Range(0, count)
            .Select(_ => GenerateProduct())
            .ToList();

    public static CreateProductDto GenerateCreateProductDto() =>
        new CreateProductDto
        {
            Title = GenerateProductTitle(),
            Price = GenerateProductPrice()
        };

    public static UpdateProductDto GenerateUpdateProductDto() =>
        new UpdateProductDto
        {
            Title = GenerateProductTitle(),
            Price = GenerateProductPrice()
        };

    private static Guid GenerateProductId() =>
        Guid.NewGuid();

    private static string GenerateProductTitle() =>
        faker.Commerce.ProductName();

    private static double GenerateProductPrice(double floor = 0.01, double ceiling = 10_000.00)
    {
        var floorInCents = (int)(floor * 100);
        var ceilingInCents = (int)(ceiling * 100);
        var priceInCents = Random.Shared.Next(floorInCents, ceilingInCents);
        return priceInCents / 100d;
    }
}

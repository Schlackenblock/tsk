using Bogus;
using Tsk.HttpApi.Products;

namespace Tsk.Tests.Products;

public static class ProductTestData
{
    private static readonly Faker faker = new();

    public static ProductEntity GenerateProduct(Action<ProductEntity>? configureProduct = null)
    {
        var product = new ProductEntity
        {
            Id = GenerateProductId(),
            Title = GenerateProductTitle(),
            Price = GenerateProductPrice()
        };

        configureProduct?.Invoke(product);

        return product;
    }

    public static List<ProductEntity> GenerateProducts(int count, Action<ProductEntity>? configureProduct = null) =>
        Enumerable
            .Range(0, count)
            .Select(_ => GenerateProduct(configureProduct))
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

    private static double GenerateProductPrice() =>
        Random.Shared.NextDouble(0.00, 10_000.00, 2);
}

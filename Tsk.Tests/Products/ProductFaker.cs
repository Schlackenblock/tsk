using Bogus;
using Tsk.HttpApi.Products;

namespace Tsk.Tests.Products;

public static class ProductFaker
{
    private static readonly Faker faker = new();

    private const double defaultMinPrice = 0.01;
    private const double defaultMaxPrice = 100.00;

    public static ProductEntity MakeEntity() =>
        new ProductEntity
        {
            Id = MakeProductId(),
            Title = MakeProductName(),
            Price = MakeProductPrice()
        };

    public static List<ProductEntity> MakeEntities(int count) =>
        Enumerable
            .Range(0, count)
            .Select(_ => MakeEntity())
            .ToList();

    public static CreateProductDto MakeCreateDto() =>
        new CreateProductDto
        {
            Title = MakeProductName(),
            Price = MakeProductPrice()
        };

    public static CreateProductDto MakeUpdateDto() =>
        new CreateProductDto
        {
            Title = MakeProductName(),
            Price = MakeProductPrice()
        };

    public static Guid MakeProductId() =>
        Guid.NewGuid();

    private static string MakeProductName() =>
        faker.Commerce.ProductName();

    private static double MakeProductPrice(double min = defaultMinPrice, double max = defaultMaxPrice)
    {
        var randomDouble = faker.Random.Double(min, max);
        return Math.Round(randomDouble, 2);
    }
}

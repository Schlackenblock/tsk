using Bogus;
using Tsk.HttpApi.Products;

namespace Tsk.Tests.Products;

public static class ProductFaker
{
    private static readonly Faker faker = new();

    private const double defaultMinPrice = 0.01;
    private const double defaultMaxPrice = 100.00;

    public static ProductEntity MakeEntity()
    {
        return new ProductEntity
        {
            Id = MakeProductId(),
            Title = MakeProductName(),
            Price = MakeProductPrice(),
            IsForSale = false
        };
    }

    public static List<ProductEntity> MakeEntities(int count)
    {
        return Enumerable
            .Range(0, count)
            .Select(_ => MakeEntity())
            .ToList();
    }

    public static CreateProductDto MakeCreateDto()
    {
        return new CreateProductDto
        {
            Title = MakeProductName(),
            Price = MakeProductPrice()
        };
    }

    public static CreateProductDto MakeUpdateDto()
    {
        return new CreateProductDto
        {
            Title = MakeProductName(),
            Price = MakeProductPrice()
        };
    }

    public static Guid MakeProductId()
    {
        return Guid.NewGuid();
    }

    private static string MakeProductName()
    {
        return faker.Commerce.ProductName();
    }

    private static double MakeProductPrice(double min = defaultMinPrice, double max = defaultMaxPrice)
    {
        var randomDouble = faker.Random.Double(min, max);
        return Math.Round(randomDouble, 2);
    }
}

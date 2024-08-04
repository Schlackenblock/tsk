using Tsk.HttpApi.Entities;

namespace Tsk.Tests.Products;

public static class TestDataGenerator
{
    public static Product GenerateProduct(int? index = null, Action<Product>? config = null)
    {
        var code = index is null ? "PRD" : $"PRD #{index}";
        var title = index is null ? "Product" : $"Product #{index}";
        var price = index ?? 0 + 0.99m;

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Code = code,
            Title = title,
            Pictures = [$"{title} Picture #1", $"{title} Picture #2"],
            Price = price,
            IsForSale = true
        };

        config?.Invoke(product);

        return product;
    }

    public static List<Product> GenerateProducts(int count, int startIndex = 1, Action<Product>? config = null)
    {
        return Enumerable
            .Range(startIndex, count)
            .Select(index =>
            {
                var product = new Product
                {
                    Id = Guid.NewGuid(),
                    Code = $"PRD #{index}",
                    Title = $"Product #{index}",
                    Pictures = [$"Product #{index} Picture #1", $"Product #{index} Picture #2"],
                    IsForSale = true,
                    Price = index + 0.99m
                };

                config?.Invoke(product);

                return product;
            })
            .ToList();
    }
}

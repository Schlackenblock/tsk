using Tsk.HttpApi.Entities;

namespace Tsk.Tests.IntegrationTests;

public static class TestDataGenerator
{
    public static Product GenerateProduct(
        int? index = null,
        string? code = null,
        string? title = null,
        List<string>? pictures = null,
        decimal? price = null,
        bool isForSale = true)
    {
        var generatedCode = index is null ? "PRD" : $"PRD #{index}";
        var generatedTitle = title ?? (index is null ? "Product" : $"Product #{index}");
        var generatedPictures = new List<string> { $"{generatedTitle} Picture #1", $"{generatedTitle} Picture #2" };
        var generatedPrice = price ?? (index ?? 0) + 0.99m;

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Code = code ?? generatedCode,
            Title = generatedTitle,
            Pictures = pictures ?? generatedPictures,
            Price = generatedPrice,
            IsForSale = isForSale
        };

        return product;
    }

    public static List<Product> GenerateProducts(
        int count,
        int startIndex = 1,
        string? code = null,
        List<string>? pictures = null,
        bool isForSale = true)
    {
        return Enumerable
            .Range(startIndex, count)
            .Select(index => GenerateProduct(index: index, code: code, pictures: pictures, isForSale: isForSale))
            .ToList();
    }

    public static Cart GenerateCart()
    {
        return GenerateCart([]);
    }

    public static Cart GenerateCart(Product product)
    {
        return GenerateCart(Enumerable.Repeat(product, 1));
    }

    public static Cart GenerateCart(IEnumerable<Product> products)
    {
        return new Cart
        {
            Id = Guid.NewGuid(),
            Products = products
                .Select((product, index) => new CartProduct
                {
                    ProductId = product.Id,
                    Quantity = index + 1
                })
                .ToList()
        };
    }

    public static Cart GenerateCart(IDictionary<Product, int> cartProducts)
    {
        return new Cart
        {
            Id = Guid.NewGuid(),
            Products = cartProducts
                .Select(cartProduct => new CartProduct
                {
                    ProductId = cartProduct.Key.Id,
                    Quantity = cartProduct.Value
                })
                .ToList()
        };
    }
}

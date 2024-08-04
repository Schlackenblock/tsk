using Tsk.HttpApi.Entities;
using Tsk.HttpApi.Products.ForAdmins;

namespace Tsk.Tests.Products;

public static class TestDataGenerator
{
    public static Product GenerateProduct(
        int? index = null,
        string? code = null,
        List<string>? pictures = null,
        bool isForSale = true)
    {
        var generatedCode = index is null ? "PRD" : $"PRD #{index}";
        var generatedTitle = index is null ? "Product" : $"Product #{index}";
        var generatedPictures = new List<string> { $"{generatedTitle} Picture #1", $"{generatedTitle} Picture #2" };
        var generatedPrice = index ?? 0 + 0.99m;

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
}

public static class ProductModelConverter
{
    public static CreateProductDto ToCreateProductDto(this Product product)
    {
        return new CreateProductDto
        {
            Code = product.Code,
            Title = product.Title,
            Pictures = product.Pictures,
            Price = product.Price
        };
    }
}

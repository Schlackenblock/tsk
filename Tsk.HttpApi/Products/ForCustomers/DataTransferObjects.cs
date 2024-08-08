using JetBrains.Annotations;
using Tsk.HttpApi.Entities;

namespace Tsk.HttpApi.Products.ForCustomers;

[PublicAPI]
public class ProductDto
{
    public required Guid Id { get; init; }
    public required string Code { get; init; }
    public required string Title { get; init; }
    public required List<string> Pictures { get; init; }
    public required decimal Price { get; init; }

    public static ProductDto FromProductEntity(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Code = product.Code,
            Title = product.Title,
            Pictures = product.Pictures,
            Price = product.Price
        };
    }
}

[PublicAPI]
public class ProductsPageDto
{
    public required List<ProductDto> Products { get; init; }
    public required int ProductsCount { get; init; }
}

[PublicAPI]
public enum ProductsOrder
{
    PriceAscending,
    PriceDescending,
    TitleAscending,
    TitleDescending
}

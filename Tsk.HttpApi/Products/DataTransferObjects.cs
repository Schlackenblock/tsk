using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using Tsk.HttpApi.Validation;

namespace Tsk.HttpApi.Products;

[PublicAPI]
public class ProductDto
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public required double Price { get; init; }
}

[PublicAPI]
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ProductOrderingOption
{
    TitleAscending,
    TitleDescending,
    PriceAscending,
    PriceDescending
}

[PublicAPI]
public class ProductsPageDto
{
    public required IReadOnlyCollection<ProductDto> Products { get; init; }
    public required int TotalProductsCount { get; init; }
    public required int PagesCount { get; set; }
}

[PublicAPI]
public class CreateProductDto
{
    [Required]
    [MaxLength(100)]
    public required string Title { get; init; }

    [Required]
    [GreaterThan(0)]
    public required double Price { get; init; }
}

[PublicAPI]
public class UpdateProductDto
{
    [Required]
    [MaxLength(100)]
    public required string Title { get; init; }

    [Required]
    [GreaterThan(0)]
    public required double Price { get; init; }
}

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Tsk.HttpApi.Validation;

namespace Tsk.HttpApi.Products;

[PublicAPI]
public class GetProductsDto
{
    [FromQuery]
    [GreaterThan(0)]
    public double? MinPrice { get; set; }

    [FromQuery]
    [GreaterThan(0)]
    public double? MaxPrice { get; set; }

    [FromQuery]
    [Required]
    public ProductsOrderingOption OrderingOption { get; set; }

    [FromQuery]
    [Required]
    [GreaterThan(0, IsExclusive = false)]
    public int PageNumber { get; set; }

    [FromQuery]
    [Required]
    [Range(1, 25)]
    public int PageSize { get; set; }
}

[PublicAPI]
public class ProductsPageDto
{
    public required IReadOnlyCollection<ProductDto> Products { get; init; }
    public required int ProductsCount { get; init; }
    public required int PagesCount { get; init; }
}

[PublicAPI]
public class ProductDto
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public required double Price { get; init; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ProductsOrderingOption
{
    TitleAscending,
    TitleDescending,
    PriceAscending,
    PriceDescending
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

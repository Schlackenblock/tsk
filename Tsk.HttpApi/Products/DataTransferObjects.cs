using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using Tsk.HttpApi.Validation;

namespace Tsk.HttpApi.Products;

[PublicAPI]
public class ProductDto
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required double Price { get; init; }
}

[PublicAPI]
public class CreateProductDto
{
    [Required]
    [MaxLength(100)]
    public required string Title { get; init; }

    [Required]
    [MaxLength(100)]
    public required string Description { get; init; }

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
    [MaxLength(100)]
    public required string Description { get; init; }

    [Required]
    [GreaterThan(0)]
    public required double Price { get; init; }
}

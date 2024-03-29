using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

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
    [RegularExpression(@"^[\w\s\.-–—]*$")]
    public required string Title { get; init; }

    [Required]
    [MaxLength(100)]
    [RegularExpression(@"^[\w\s\.\d]*")]
    public required string Description { get; init; }

    [Required]
    public required double Price { get; init; }
}

[PublicAPI]
public class UpdateProductDto
{
    [Required]
    [MaxLength(100)]
    [RegularExpression(@"^[\w\s\.-–—]*$")]
    public required string Title { get; init; }

    [Required]
    [MaxLength(100)]
    [RegularExpression(@"^[\w\s\.\d]*")]
    public required string Description { get; init; }

    [Required]
    public required double Price { get; init; }
}

using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using Tsk.HttpApi.Validation;

namespace Tsk.HttpApi.Products.ForAdmins;

[PublicAPI]
public class ProductDto
{
    public required Guid Id { get; init; }
    public required string Code { get; init; }
    public required string Title { get; init; }
    public required decimal Price { get; init; }
    public required bool IsForSale { get; init; }
}

[PublicAPI]
public class CreateProductDto
{
    [Required]
    [MaxLength(100)]
    public required string Title { get; init; }

    [Required]
    [MaxLength(50)]
    public required string Code { get; init; }

    [Required]
    [Price]
    public required decimal Price { get; init; }
}

[PublicAPI]
public class UpdateProductDto
{
    [Required]
    [MaxLength(100)]
    public required string Title { get; init; }

    [Required]
    [MaxLength(50)]
    public required string Code { get; init; }

    [Required]
    [Price]
    public required decimal Price { get; init; }
}

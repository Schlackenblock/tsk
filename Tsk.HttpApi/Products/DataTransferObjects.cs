using JetBrains.Annotations;

namespace Tsk.HttpApi.Products;

[PublicAPI]
public class ProductDto
{
    public Guid Id { get; init; }
    public string Title { get; init; }
    public string Description { get; init; }
    public double Price { get; init; }
}

[PublicAPI]
public class CreateProductDto
{
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required double Price { get; init; }
}

[PublicAPI]
public record UpdateProductDto
{
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required double Price { get; init; }
}

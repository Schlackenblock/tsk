using JetBrains.Annotations;

namespace Tsk.HttpApi.Products;

[PublicAPI]
public record ProductDto
{
    public Guid Id { get; init; }
    public string Title { get; init; }
    public string Description { get; init; }
    public double Price { get; init; }
}

[PublicAPI]
public class CreateProductDto
{
    public string Title { get; init; }
    public string Description { get; init; }
    public double Price { get; init; }
}

[PublicAPI]
public record UpdateProductDto
{
    public string Title { get; init; }
    public string Description { get; init; }
    public double Price { get; init; }
}

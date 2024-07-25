using JetBrains.Annotations;

namespace Tsk.HttpApi.Products.ForCustomers;

[PublicAPI]
public class ProductDto
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public required double Price { get; init; }
}
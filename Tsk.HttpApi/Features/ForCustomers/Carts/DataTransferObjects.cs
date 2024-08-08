using JetBrains.Annotations;

namespace Tsk.HttpApi.Features.ForCustomers.Carts;

[PublicAPI]
public class CartDto
{
    public required Guid Id { get; init; }
    public required List<CartProductDto> Products { get; init; }
}

[PublicAPI]
public class CartProductDto
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public required string? Picture { get; init; }
    public required decimal Price { get; init; }
    public required int Quantity { get; init; }
}

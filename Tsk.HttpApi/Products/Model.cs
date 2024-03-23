namespace Tsk.HttpApi.Products;

internal class ProductEntity
{
    public Guid Id { get; init; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required double Price { get; set; }

    // TODO: Remove immediately.
    public int TestProperty { get; set; }
}

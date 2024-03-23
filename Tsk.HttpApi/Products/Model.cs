namespace Tsk.HttpApi.Products;

public class ProductEntity
{
    public Guid Id { get; init; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required double Price { get; set; }
}

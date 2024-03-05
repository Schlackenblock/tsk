namespace Tsk.HttpApi.Controllers;

internal class Product
{
    public Guid Id { get; init; }
    public required string Description { get; set; }
    public required string Name { get; set; }
    public required double Price { get; set; }
}

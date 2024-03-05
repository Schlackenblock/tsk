namespace Tsk.HttpApi.Products;

internal class Product
{
    public Guid Id { get; init; }
    public required string Title { get; set; }
    public required string Name { get; set; }
    public required string Type { get; set; }
    public double? Length { get; set; }
    public double? Width { get; set; }
    public double? Height { get; set; }
    public required double Price { get; set; }
    public double? Discount { get; set; }
}

using System.ComponentModel.DataAnnotations.Schema;

namespace Tsk.HttpApi.Entities;

[Table("products")]
public class ProductEntity
{
    [Column("id")]
    public Guid Id { get; init; }

    [Column("title")]
    public required string Title { get; set; }

    [Column("price", TypeName = "numeric(12,2)")]
    public required decimal Price { get; set; }

    [Column("is_for_sale")]
    public required bool IsForSale { get; set; }
}

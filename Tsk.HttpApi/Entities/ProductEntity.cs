using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tsk.HttpApi.Entities;

public class ProductEntity
{
    public Guid Id { get; init; }
    public required string Title { get; set; }
    public required decimal Price { get; set; }
    public required bool IsForSale { get; set; }
}

internal class ProductEntityTypeConfiguration : IEntityTypeConfiguration<ProductEntity>
{
    public void Configure(EntityTypeBuilder<ProductEntity> productEntity)
    {
        productEntity.ToTable("products");

        productEntity
            .Property(product => product.Id)
            .HasColumnName("id");

        productEntity
            .Property(product => product.Title)
            .HasColumnName("title");

        productEntity
            .Property(product => product.Price)
            .HasColumnType("numeric(12,2)")
            .HasColumnName("price");

        productEntity
            .Property(product => product.IsForSale)
            .HasColumnName("is_for_sale");
    }
}

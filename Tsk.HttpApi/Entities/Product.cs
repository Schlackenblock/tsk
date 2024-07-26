using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tsk.HttpApi.Entities;

public class Product
{
    public required Guid Id { get; init; }
    public required string Code { get; init; }
    public required string Title { get; set; }
    public required decimal Price { get; set; }
    public required bool IsForSale { get; set; }
}

internal class ProductEntityTypeConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> productEntity)
    {
        productEntity.ToTable("products");

        productEntity
            .HasKey(product => product.Id)
            .HasName("pk_products");

        productEntity
            .Property(product => product.Id)
            .HasColumnName("id");

        productEntity
            .Property(product => product.Code)
            .HasColumnName("code");

        productEntity
            .HasIndex(product => product.Code)
            .IsUnique();

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

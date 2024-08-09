using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tsk.HttpApi.Entities;

public class Cart
{
    public required Guid Id { get; init; }
    public required List<CartProduct> Products { get; init; }
}

public class CartProduct
{
    public required Guid ProductId { get; init; }
    public required int Quantity { get; set; }
}

public class CartEntityTypeConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> cartEntity)
    {
        cartEntity.ToTable("carts");

        cartEntity
            .HasKey(cart => cart.Id)
            .HasName("pk_carts");

        cartEntity
            .Property(cart => cart.Id)
            .HasColumnName("id");

        cartEntity.OwnsMany(
            cart => cart.Products,
            products => products.ToJson("products")
        );
    }
}

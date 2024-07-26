using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tsk.HttpApi.Entities;

public class Cart
{
    public Guid Id { get; init; }

    public List<ProductInCart>? Products { get; init; }
}

public class ProductInCart
{
    public required Guid Id { get; init; }

    public required Guid ProductId { get; init; }
    public Product? Product { get; init; }

    public required int Quantity { get; set; }

    public required Guid CartId { get; init; }
    public Cart? Cart { get; init; }
}

internal class CartEntityTypeConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> cartEntity)
    {
        cartEntity.ToTable("carts");

        cartEntity
            .Property(cart => cart.Id)
            .HasColumnName("id");

        cartEntity
            .HasMany(cart => cart.Products)
            .WithOne(productInCart => productInCart.Cart)
            .HasForeignKey(productInCart => productInCart.CartId);
    }
}

internal class ProductInCartEntityTypeConfiguration : IEntityTypeConfiguration<ProductInCart>
{
    public void Configure(EntityTypeBuilder<ProductInCart> productInCartEntity)
    {
        productInCartEntity.ToTable("product_in_cart");

        productInCartEntity
            .Property(productInCart => productInCart.Id)
            .HasColumnName("id");

        productInCartEntity
            .Property(productInCart => productInCart.ProductId)
            .HasColumnName("product_id");

        productInCartEntity
            .HasOne(productInCart => productInCart.Product)
            .WithMany()
            .HasForeignKey(productInCart => productInCart.ProductId);

        productInCartEntity
            .Property(productInCart => productInCart.Quantity)
            .HasColumnName("quantity");

        productInCartEntity
            .Property(productInCart => productInCart.CartId)
            .HasColumnName("cart_id");
    }
}

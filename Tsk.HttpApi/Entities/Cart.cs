using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tsk.HttpApi.Entities;

public class Cart
{
    public Guid Id { get; init; }

    public List<CartProduct>? Products { get; init; }
}

public class CartProduct
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
            .HasKey(cart => cart.Id)
            .HasName("pk_carts");

        cartEntity
            .Property(cart => cart.Id)
            .HasColumnName("id");
    }
}

internal class CartProductEntityTypeConfiguration : IEntityTypeConfiguration<CartProduct>
{
    public void Configure(EntityTypeBuilder<CartProduct> cartProductEntity)
    {
        cartProductEntity.ToTable("cart_products");

        cartProductEntity
            .HasKey(cartProduct => cartProduct.Id)
            .HasName("pk_cart_products");

        cartProductEntity
            .Property(cartProduct => cartProduct.Id)
            .HasColumnName("id");

        cartProductEntity
            .Property(cartProduct => cartProduct.ProductId)
            .HasColumnName("product_id");

        cartProductEntity
            .HasOne(cartProduct => cartProduct.Product)
            .WithMany()
            .HasForeignKey(cartProduct => cartProduct.ProductId)
            .HasConstraintName("fk_cart_products_products_product_id");

        cartProductEntity
            .HasIndex(cartProduct => cartProduct.ProductId)
            .HasDatabaseName("ix_cart_products_product_id");

        cartProductEntity
            .Property(cartProduct => cartProduct.Quantity)
            .HasColumnName("quantity");

        cartProductEntity
            .Property(cartProduct => cartProduct.CartId)
            .HasColumnName("cart_id");

        cartProductEntity
            .HasOne(cartProduct => cartProduct.Cart)
            .WithMany(cart => cart.Products)
            .HasForeignKey(cartProduct => cartProduct.CartId)
            .HasConstraintName("fk_cart_products_carts_cart_id");

        cartProductEntity
            .HasIndex(cartProduct => cartProduct.CartId)
            .HasDatabaseName("ix_cart_products_cart_id");
    }
}

using Tsk.HttpApi.Entities;

namespace Tsk.Tests.IntegrationTests.ForCustomers.Carts;

public class DecreaseCartProductQuantityTestSuite : IntegrationTestSuiteBase
{
    [Fact]
    public async Task DecreaseCartProductQuantity_WhenCartHasSpecifiedProduct_ShouldDecreaseSpecifiedProductQuantity()
    {
        var anotherProduct = TestDataGenerator.GenerateProduct(index: 1);
        var specifiedProduct = TestDataGenerator.GenerateProduct(index: 2);
        await SeedInitialDataAsync([specifiedProduct, anotherProduct]);

        var initialCart = TestDataGenerator.GenerateCart(new Dictionary<Product, int>
        {
            { anotherProduct, 1 },
            { specifiedProduct, 2 }
        });
        await SeedInitialDataAsync(initialCart);

        var response = await HttpClient.PostAsync($"/carts/{initialCart.Id}/products/{specifiedProduct.Id}/decrease-quantity", null);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await AssertDbStateAsync(async dbContext =>
        {
            var updatedCart = await dbContext.Carts.SingleAsync();
            updatedCart.Id.Should().Be(initialCart.Id);
            updatedCart.Products.Should().BeEquivalentTo(new[]
            {
                new CartProduct { ProductId = anotherProduct.Id, Quantity = 1 },
                new CartProduct { ProductId = specifiedProduct.Id, Quantity = 1 }
            });
        });
    }

    [Fact]
    public async Task DecreaseCartProductQuantity_WhenCartHasSpecifiedProductWithQuantityOne_ShouldReturnBadRequest()
    {
        var product = TestDataGenerator.GenerateProduct(index: 2);
        await SeedInitialDataAsync(product);

        var initialCart = TestDataGenerator.GenerateCart(new Dictionary<Product, int>
        {
            { product, 1 }
        });
        await SeedInitialDataAsync(initialCart);

        var response = await HttpClient.PostAsync($"/carts/{initialCart.Id}/products/{product.Id}/decrease-quantity", null);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        await AssertDbStateAsync(async dbContext =>
        {
            var updatedCart = await dbContext.Carts.SingleAsync();
            updatedCart.Id.Should().Be(initialCart.Id);
            updatedCart.Products.Should().BeEquivalentTo(new[]
            {
                new CartProduct { ProductId = product.Id, Quantity = 1 }
            });
        });
    }

    [Fact]
    public async Task DecreaseCartProductQuantity_WhenCartDoesNotHaveSpecifiedProduct_ShouldReturnNotFound()
    {
        var productInCart = TestDataGenerator.GenerateProduct(index: 1);
        var productNotInCart = TestDataGenerator.GenerateProduct(index: 2);
        await SeedInitialDataAsync([productInCart, productNotInCart]);

        var cart = TestDataGenerator.GenerateCart(productInCart);

        var response = await HttpClient.PostAsync($"/carts/{cart.Id}/products/{productNotInCart.Id}/decrease-quantity", null);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DecreaseCartProductQuantity_WhenCartDoesNotExist_ShouldReturnNotFound()
    {
        var product = TestDataGenerator.GenerateProduct();
        await SeedInitialDataAsync(product);

        var notExistingCartId = Guid.NewGuid();

        var response = await HttpClient.PostAsync($"/carts/{notExistingCartId}/products/{product.Id}/decrease-quantity", null);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

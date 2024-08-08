using Tsk.HttpApi.Entities;

namespace Tsk.Tests.IntegrationTests.ForCustomers.Carts;

public class IncreaseCartProductQuantityTestSuite : IntegrationTestSuiteBase
{
    [Fact]
    public async Task IncreaseCartProductQuantity_WhenCartHasSpecifiedProduct_ShouldIncreaseSpecifiedProductQuantity()
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

        var response = await HttpClient.PostAsync($"/carts/{initialCart.Id}/products/{specifiedProduct.Id}/increase-quantity", null);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await AssertDbStateAsync(async dbContext =>
        {
            var updatedCart = await dbContext.Carts.SingleAsync();
            updatedCart.Id.Should().Be(initialCart.Id);
            updatedCart.Products.Should().BeEquivalentTo(new[]
            {
                new CartProduct { ProductId = anotherProduct.Id, Quantity = 1 },
                new CartProduct { ProductId = specifiedProduct.Id, Quantity = 3 }
            });
        });
    }

    [Fact]
    public async Task IncreaseCartProductQuantity_WhenCartDoesNotHaveSpecifiedProduct_ShouldReturnNotFound()
    {
        var productInCart = TestDataGenerator.GenerateProduct(index: 1);
        var productNotInCart = TestDataGenerator.GenerateProduct(index: 2);
        await SeedInitialDataAsync([productInCart, productNotInCart]);

        var cart = TestDataGenerator.GenerateCart(productInCart);

        var response = await HttpClient.PostAsync($"/carts/{cart.Id}/products/{productNotInCart.Id}/increase-quantity", null);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task IncreaseCartProductQuantity_WhenCartDoesNotExist_ShouldReturnNotFound()
    {
        var product = TestDataGenerator.GenerateProduct();
        await SeedInitialDataAsync(product);

        var notExistingCartId = Guid.NewGuid();

        var response = await HttpClient.PostAsync($"/carts/{notExistingCartId}/products/{product.Id}/increase-quantity", null);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

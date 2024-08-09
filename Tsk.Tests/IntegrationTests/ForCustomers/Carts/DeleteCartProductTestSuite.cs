using Tsk.HttpApi.Entities;

namespace Tsk.Tests.IntegrationTests.ForCustomers.Carts;

public class DeleteCartProductTestSuite : IntegrationTestSuiteBase
{
    [Fact]
    public async Task DeleteCartProduct_WhenCartHasSpecifiedProduct_ShouldRemoveCartProduct()
    {
        var specifiedProduct = TestDataGenerator.GenerateProduct(index: 1);
        var anotherProduct = TestDataGenerator.GenerateProduct(index: 2);
        await SeedInitialDataAsync([specifiedProduct, anotherProduct]);

        var initialCart = TestDataGenerator.GenerateCart(new Dictionary<Product, int>
        {
            { specifiedProduct, 3 },
            { anotherProduct, 2 }
        });
        await SeedInitialDataAsync(initialCart);

        var response = await HttpClient.DeleteAsync($"/carts/{initialCart.Id}/products/{specifiedProduct.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await AssertDbStateAsync(async dbContext =>
        {
            var updatedCart = await dbContext.Carts.SingleAsync();
            updatedCart.Should().BeEquivalentTo(new Cart
            {
                Id = initialCart.Id,
                Products = [new CartProduct { ProductId = anotherProduct.Id, Quantity = 2 }]
            });
        });
    }

    [Fact]
    public async Task DeleteCartProduct_WhenCartDoesNotHaveSpecifiedProduct_ShouldReturnNotFound()
    {
        var specifiedProduct = TestDataGenerator.GenerateProduct(index: 1);
        var anotherProduct = TestDataGenerator.GenerateProduct(index: 2);
        await SeedInitialDataAsync([specifiedProduct, anotherProduct]);

        var initialCart = TestDataGenerator.GenerateCart(anotherProduct);
        await SeedInitialDataAsync(initialCart);

        var response = await HttpClient.DeleteAsync($"/carts/{initialCart.Id}/products/{specifiedProduct.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        await AssertDbStateAsync(async dbContext =>
        {
            var updatedCart = await dbContext.Carts.SingleAsync();
            updatedCart.Should().BeEquivalentTo(initialCart);
        });
    }

    [Fact]
    public async Task DeleteCartProduct_WhenCartDoesNotExist_ShouldReturnNotFound()
    {
        var product = TestDataGenerator.GenerateProduct();
        await SeedInitialDataAsync(product);

        var notExistingCartId = Guid.NewGuid();

        var response = await HttpClient.DeleteAsync($"/carts/{notExistingCartId}/products/{product.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

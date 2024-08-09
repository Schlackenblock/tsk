using Tsk.HttpApi.Entities;

namespace Tsk.Tests.IntegrationTests.ForCustomers.Carts;

public class AddProductToCartTestSuite : IntegrationTestSuiteBase
{
    [Fact]
    public async Task AddProductToCart_WhenCartDoesNotHaveSpecifiedProduct_ShouldSucceed()
    {
        var anotherProduct = TestDataGenerator.GenerateProduct(index: 1);
        var specifiedProduct = TestDataGenerator.GenerateProduct(index: 2);
        await SeedInitialDataAsync([anotherProduct, specifiedProduct]);

        var initialCart = TestDataGenerator.GenerateCart(new Dictionary<Product, int>
        {
            { anotherProduct, 2 }
        });
        await SeedInitialDataAsync(initialCart);

        var response = await HttpClient.PostAsync($"/carts/{initialCart.Id}/products/{specifiedProduct.Id}/add-to-cart", null);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await AssertDbStateAsync(async dbContext =>
        {
            var updatedCart = await dbContext.Carts.SingleAsync();
            updatedCart.Should().BeEquivalentTo(new Cart
            {
                Id = initialCart.Id,
                Products =
                [
                    new CartProduct { ProductId = anotherProduct.Id, Quantity = 2 },
                    new CartProduct { ProductId = specifiedProduct.Id, Quantity = 1 }
                ]
            });
        });
    }

    [Fact]
    public async Task AddProductToCart_WhenProductIsNotForSale_ShouldReturnBadRequest()
    {
        var anotherProduct = TestDataGenerator.GenerateProduct(index: 1, isForSale: true);
        var productNotForSale = TestDataGenerator.GenerateProduct(index: 2, isForSale: false);
        await SeedInitialDataAsync([anotherProduct, productNotForSale]);

        var initialCart = TestDataGenerator.GenerateCart(anotherProduct);
        await SeedInitialDataAsync(initialCart);

        var response = await HttpClient.PostAsync($"/carts/{initialCart.Id}/products/{productNotForSale.Id}/add-to-cart", null);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        await AssertDbStateAsync(async dbContext =>
        {
            var updatedCart = await dbContext.Carts.SingleAsync();
            updatedCart.Should().BeEquivalentTo(initialCart);
        });
    }

    [Fact]
    public async Task AddProductToCart_WhenCartAlreadyHasSpecifiedProduct_ShouldReturnBadRequest()
    {
        var anotherProduct = TestDataGenerator.GenerateProduct(index: 1);
        var specifiedProduct = TestDataGenerator.GenerateProduct(index: 2);
        await SeedInitialDataAsync([anotherProduct, specifiedProduct]);

        var initialCart = TestDataGenerator.GenerateCart(new Dictionary<Product, int>
        {
            { anotherProduct, 2 },
            { specifiedProduct, 3 }
        });
        await SeedInitialDataAsync(initialCart);

        var response = await HttpClient.PostAsync($"/carts/{initialCart.Id}/products/{specifiedProduct.Id}/add-to-cart", null);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        await AssertDbStateAsync(async dbContext =>
        {
            var updatedCart = await dbContext.Carts.SingleAsync();
            updatedCart.Should().BeEquivalentTo(initialCart);
        });
    }

    [Fact]
    public async Task AddProductToCart_WhenSpecifiedProductDoesNotExist_ShouldReturnNotFound()
    {
        var anotherProduct = TestDataGenerator.GenerateProduct();
        await SeedInitialDataAsync(anotherProduct);

        var initialCart = TestDataGenerator.GenerateCart(anotherProduct);
        await SeedInitialDataAsync(initialCart);

        var notExistingProductId = Guid.NewGuid();

        var response = await HttpClient.PostAsync($"/carts/{initialCart.Id}/products/{notExistingProductId}/add-to-cart", null);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        await AssertDbStateAsync(async dbContext =>
        {
            var updatedCart = await dbContext.Carts.SingleAsync();
            updatedCart.Should().BeEquivalentTo(initialCart);
        });
    }

    [Fact]
    public async Task AddProductToCart_WhenCartDoesNotExist_ShouldReturnNotFound()
    {
        var product = TestDataGenerator.GenerateProduct();
        await SeedInitialDataAsync(product);

        var notExistingCartId = Guid.NewGuid();

        var response = await HttpClient.PostAsync($"/carts/{notExistingCartId}/products/{product.Id}/add-to-cart", null);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

using Tsk.HttpApi.Entities;
using Tsk.HttpApi.Features.ForCustomers.Carts;

namespace Tsk.Tests.IntegrationTests.ForCustomers.Carts;

public class GetCartTestSuite : IntegrationTestSuiteBase
{
    [Fact]
    public async Task GetCart_WhenCartReferencesNotExistingProducts_ShouldReturnInternalServerError()
    {
        var notExistingProductId = Guid.NewGuid();
        var cart = new Cart
        {
            Id = Guid.NewGuid(),
            Products =
            [
                new CartProduct { ProductId = notExistingProductId, Quantity = 1 }
            ]
        };
        await SeedInitialDataAsync(cart);

        var response = await HttpClient.GetAsync($"/carts/{cart.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task GetCart_WhenCartHasProducts_ShouldReturnCartWithProducts()
    {
        var products = TestDataGenerator.GenerateProducts(count: 2);
        await SeedInitialDataAsync(products);

        var cart = TestDataGenerator.GenerateCart(products);
        await SeedInitialDataAsync(cart);

        var response = await HttpClient.GetAsync($"/carts/{cart.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var expectedCartProductDtos = cart.Products.Select(cartProduct =>
        {
            var product = products.Single(product => product.Id == cartProduct.ProductId);
            return new CartProductDto
            {
                Id = product.Id,
                Title = product.Title,
                Picture = product.Pictures.First(),
                Price = product.Price,
                Quantity = cartProduct.Quantity
            };
        });

        var cartDto = await response.Content.ReadFromJsonAsync<CartDto>();
        cartDto!.Id.Should().Be(cart.Id);
        cartDto.Products.Should().BeEquivalentTo(expectedCartProductDtos);
    }

    [Fact]
    public async Task GetCart_WhenCartDoesNotHaveProducts_ShouldReturnEmptyCart()
    {
        var cart = TestDataGenerator.GenerateCart();
        await SeedInitialDataAsync(cart);

        var response = await HttpClient.GetAsync($"/carts/{cart.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var cartDto = await response.Content.ReadFromJsonAsync<CartDto>();
        cartDto!.Id.Should().Be(cart.Id);
        cartDto.Products.Should().BeEmpty();
    }

    [Fact]
    public async Task GetCart_WhenCartDoesNotExist_ShouldReturnNotFound()
    {
        var notExistingCartId = Guid.NewGuid();

        var response = await HttpClient.GetAsync($"/carts/{notExistingCartId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}


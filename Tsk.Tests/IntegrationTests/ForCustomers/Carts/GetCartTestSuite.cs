using Tsk.HttpApi.Entities;
using Tsk.HttpApi.Features.ForCustomers.Carts;

namespace Tsk.Tests.IntegrationTests.ForCustomers.Carts;

public class GetCartTestSuite : IntegrationTestSuiteBase
{
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
                IsForSale = product.IsForSale,
                Quantity = cartProduct.Quantity
            };
        });

        var cartDto = await response.Content.ReadFromJsonAsync<CartDto>();
        cartDto!.Id.Should().Be(cart.Id);
        cartDto.Products.Should().BeEquivalentTo(expectedCartProductDtos);
    }

    [Fact]
    public async Task GetCart_WhenCartHasProductWhichIsNotForSale_ShouldReturnProductWithoutPrice()
    {
        var productForSale = TestDataGenerator.GenerateProduct(index: 1, isForSale: true);
        var productNotForSale = TestDataGenerator.GenerateProduct(index: 2, isForSale: false);
        await SeedInitialDataAsync([productForSale, productNotForSale]);

        var cart = TestDataGenerator.GenerateCart(new Dictionary<Product, int>
        {
            { productForSale, 2 },
            { productNotForSale, 3 }
        });
        await SeedInitialDataAsync(cart);

        var response = await HttpClient.GetAsync($"/carts/{cart.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var cartDto = await response.Content.ReadFromJsonAsync<CartDto>();
        cartDto.Should().BeEquivalentTo(new CartDto
        {
            Id = cart.Id,
            Products =
            [
                new CartProductDto
                {
                    Id = productForSale.Id,
                    Title = productForSale.Title,
                    Picture = productForSale.Pictures.First(),
                    Price = productForSale.Price,
                    IsForSale = true,
                    Quantity = 2
                },
                new CartProductDto
                {
                    Id = productNotForSale.Id,
                    Title = productNotForSale.Title,
                    Picture = productNotForSale.Pictures.First(),
                    Price = null,
                    IsForSale = false,
                    Quantity = 3
                }
            ]
        });
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
    public async Task GetCart_WhenCartDoesNotExist_ShouldReturnNotFound()
    {
        var notExistingCartId = Guid.NewGuid();

        var response = await HttpClient.GetAsync($"/carts/{notExistingCartId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}


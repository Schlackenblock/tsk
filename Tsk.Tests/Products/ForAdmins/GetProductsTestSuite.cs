using Tsk.HttpApi.Products;
using Tsk.HttpApi.Products.ForAdmins;

namespace Tsk.Tests.Products.ForAdmins;

public class GetProductsTestSuite : TestSuiteBase
{
    [Fact]
    public async Task GetProducts_WhenManyExist_ShouldBothForSaleAndNotForSale()
    {
        var existingProducts = new[]
        {
            new ProductEntity
            {
                Id = Guid.NewGuid(),
                Title = "Product for sale",
                Price = 9.99,
                IsForSale = true
            },
            new ProductEntity
            {
                Id = Guid.NewGuid(),
                Title = "Product not for sale",
                Price = 8.99,
                IsForSale = false
            }
        };
        Context.Products.AddRange(existingProducts);
        await Context.SaveChangesAsync();

        var response = await HttpClient.GetAsync("/management/products");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productDtos = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        productDtos.Should().BeEquivalentTo(existingProducts);
    }

    [Fact]
    public async Task GetProducts_WhenNoneExist_ShouldReturnNone()
    {
        var response = await HttpClient.GetAsync("/management/products");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productDtos = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        productDtos.Should().BeEmpty();
    }
}
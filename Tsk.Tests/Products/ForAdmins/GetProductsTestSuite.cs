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

        await using (var dbContext = CreateDbContext())
        {
            dbContext.Products.AddRange(existingProducts);
            await dbContext.SaveChangesAsync();
        }

        using var httpClient = CreateHttpClient();
        var response = await httpClient.GetAsync("/management/products");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productDtos = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        productDtos.Should().BeEquivalentTo(existingProducts);
    }

    [Fact]
    public async Task GetProducts_WhenNoneExist_ShouldReturnNone()
    {
        using var httpClient = CreateHttpClient();
        var response = await httpClient.GetAsync("/management/products");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productDtos = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        productDtos.Should().BeEmpty();
    }
}

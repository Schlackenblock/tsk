using Tsk.HttpApi.Products;
using Tsk.HttpApi.Products.ForCustomers;

namespace Tsk.Tests.Products.ForCustomers;

public class GetProductsTestSuite : TestSuiteBase
{
    [Fact]
    public async Task GetProducts_WhenManyExist_ShouldReturnMany()
    {
        var productNotForSale = new ProductEntity
        {
            Id = Guid.NewGuid(),
            Title = "Product not for sale",
            Price = 9.99,
            IsForSale = false
        };
        var productForSale = new ProductEntity
        {
            Id = Guid.NewGuid(),
            Title = "Product for sale",
            Price = 8.99,
            IsForSale = true
        };

        await using (var dbContext = CreateDbContext())
        {
            dbContext.Products.AddRange([productNotForSale, productForSale]);
            await dbContext.SaveChangesAsync();
        }

        using var httpClient = CreateHttpClient();
        var response = await httpClient.GetAsync("/products");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productDtos = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        productDtos!.Single().Should().BeEquivalentTo(new
        {
            productForSale.Id,
            productForSale.Title,
            productForSale.Price
        });
    }

    [Fact]
    public async Task GetProducts_WhenNoneExist_ShouldReturnNone()
    {
        using var httpClient = CreateHttpClient();
        var response = await httpClient.GetAsync("/products");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productDtos = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        productDtos.Should().BeEmpty();
    }
}

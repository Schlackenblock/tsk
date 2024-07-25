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
            Price = 9.99m,
            IsForSale = false
        };
        var productForSale = new ProductEntity
        {
            Id = Guid.NewGuid(),
            Title = "Product for sale",
            Price = 8.99m,
            IsForSale = true
        };

        await CallDbAsync(async dbContext =>
        {
            dbContext.Products.AddRange([productNotForSale, productForSale]);
            await dbContext.SaveChangesAsync();
        });

        var response = await HttpClient.GetAsync("/products");
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
        var response = await HttpClient.GetAsync("/products");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productDtos = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        productDtos.Should().BeEmpty();
    }
}

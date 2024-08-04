using Tsk.HttpApi.Entities;
using Tsk.HttpApi.Products.ForAdmins;

namespace Tsk.Tests.Products.ForAdmins;

public class GetProductsTestSuite : IntegrationTestSuiteBase
{
    [Fact]
    public async Task GetProducts_WhenManyExist_ShouldBothForSaleAndNotForSale()
    {
        var existingProducts = new[]
        {
            new Product { Id = Guid.NewGuid(), Code = "P1", Title = "For sale", Pictures = ["For sale Picture 1", "For sale Picture 2"], Price = 9.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Code = "P2", Title = "Not for sale", Pictures = ["Not for sale Picture 1", "Not for sale Picture 2"], Price = 8.99m, IsForSale = false }
        };

        await CallDbAsync(async dbContext =>
        {
            dbContext.Products.AddRange(existingProducts);
            await dbContext.SaveChangesAsync();
        });

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

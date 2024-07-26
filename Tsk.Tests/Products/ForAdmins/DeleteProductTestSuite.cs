using Tsk.HttpApi.Entities;

namespace Tsk.Tests.Products.ForAdmins;

public class DeleteProductTestSuite : IntegrationTestSuiteBase
{
    [Fact]
    public async Task DeleteProduct_WhenProductForSaleExists_ShouldSucceed()
    {
        var product = new Product { Id = Guid.NewGuid(), Code = "P", Title = "P", Price = 9.99m, IsForSale = true };

        await CallDbAsync(async dbContext =>
        {
            dbContext.Products.Add(product);
            await dbContext.SaveChangesAsync();
        });

        var response = await HttpClient.DeleteAsync($"/management/products/{product.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await CallDbAsync(async dbContext =>
        {
            var productsExist = await dbContext.Products.AnyAsync();
            productsExist.Should().BeFalse();
        });
    }
    [Fact]
    public async Task DeleteProduct_WhenProductNotForSaleExists_ShouldSucceed()
    {
        var product = new Product { Id = Guid.NewGuid(), Code = "P", Title = "P", Price = 9.99m, IsForSale = true };

        await CallDbAsync(async dbContext =>
        {
            dbContext.Products.Add(product);
            await dbContext.SaveChangesAsync();
        });

        var response = await HttpClient.DeleteAsync($"/management/products/{product.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await CallDbAsync(async dbContext =>
        {
            var productsExist = await dbContext.Products.AnyAsync();
            productsExist.Should().BeFalse();
        });
    }

    [Fact]
    public async Task DeleteProduct_WhenProductDoesNotExist_ShouldFail()
    {
        var notExistingProductId = Guid.NewGuid();
        var response = await HttpClient.DeleteAsync($"/management/products/{notExistingProductId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

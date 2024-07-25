using Tsk.HttpApi.Products;

namespace Tsk.Tests.Products.ForAdmins;

public class DeleteProductTestSuite : TestSuiteBase
{
    [Fact]
    public async Task DeleteProduct_WhenProductForSaleExists_ShouldSucceed()
    {
        var existingProduct = new ProductEntity
        {
            Id = Guid.NewGuid(),
            Title = "Product",
            Price = 9.99,
            IsForSale = true
        };

        await CallDbAsync(async dbContext =>
        {
            dbContext.Products.Add(existingProduct);
            await dbContext.SaveChangesAsync();
        });

        var response = await HttpClient.DeleteAsync($"/management/products/{existingProduct.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await CallDbAsync(async dbContext =>
        {
            var persistedProducts = await dbContext.Products.ToListAsync();
            persistedProducts.Should().BeEmpty();
        });
    }
    [Fact]
    public async Task DeleteProduct_WhenProductNotForSaleExists_ShouldSucceed()
    {
        var existingProduct = new ProductEntity
        {
            Id = Guid.NewGuid(),
            Title = "Product",
            Price = 9.99,
            IsForSale = false
        };

        await CallDbAsync(async dbContext =>
        {
            dbContext.Products.Add(existingProduct);
            await dbContext.SaveChangesAsync();
        });

        var response = await HttpClient.DeleteAsync($"/management/products/{existingProduct.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await CallDbAsync(async dbContext =>
        {
            var persistedProducts = await dbContext.Products.ToListAsync();
            persistedProducts.Should().BeEmpty();
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

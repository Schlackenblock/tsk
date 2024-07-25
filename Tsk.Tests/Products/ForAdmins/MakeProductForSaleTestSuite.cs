using Tsk.HttpApi.Products;

namespace Tsk.Tests.Products.ForAdmins;

public class MakeProductForSaleTestSuite : IntegrationTestSuiteBase
{
    [Fact]
    public async Task MakeProductForSale_WhenProductNotForSale_ShouldSucceed()
    {
        var initialProduct = new ProductEntity
        {
            Id = Guid.NewGuid(),
            Title = "Product",
            Price = 9.99m,
            IsForSale = false
        };

        await CallDbAsync(async dbContext =>
        {
            dbContext.Products.Add(initialProduct);
            await dbContext.SaveChangesAsync();
        });

        var response = await HttpClient.PutAsync($"/management/products/{initialProduct.Id}/make-for-sale", null);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await CallDbAsync(async dbContext =>
        {
            var updatedProduct = await dbContext.Products.SingleAsync();
            updatedProduct.Should().BeEquivalentTo(new
            {
                initialProduct.Id,
                initialProduct.Title,
                initialProduct.Price,
                IsForSale = true
            });
        });
    }

    [Fact]
    public async Task MakeProductForSale_WhenProductAlreadyForSale_ShouldReturnBadRequest()
    {
        var productForSale = new ProductEntity
        {
            Id = Guid.NewGuid(),
            Title = "Product",
            Price = 9.99m,
            IsForSale = true
        };

        await CallDbAsync(async dbContext =>
        {
            dbContext.Products.Add(productForSale);
            await dbContext.SaveChangesAsync();
        });

        var response = await HttpClient.PutAsync($"/management/products/{productForSale.Id}/make-for-sale", null);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task MakeProductForSale_WhenProductDoesNotExist_ShouldReturnNotFound()
    {
        var notExistingProductId = Guid.NewGuid();
        var response = await HttpClient.PutAsync($"/management/products/{notExistingProductId}/make-for-sale", null);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

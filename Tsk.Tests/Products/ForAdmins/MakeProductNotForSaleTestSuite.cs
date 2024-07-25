using Tsk.HttpApi.Products;

namespace Tsk.Tests.Products.ForAdmins;

public class MakeProductNotForSaleTestSuite : TestSuiteBase
{
    [Fact]
    public async Task MakeProductNotForSaleTestSuite_WhenProductForSale_ShouldSucceed()
    {
        var initialProduct = new ProductEntity
        {
            Id = Guid.NewGuid(),
            Title = "Product",
            Price = 9.99,
            IsForSale = true
        };

        await using (var dbContext = CreateDbContext())
        {
            dbContext.Products.Add(initialProduct);
            await dbContext.SaveChangesAsync();
        }

        using var httpClient = CreateHttpClient();
        var response = await httpClient.PutAsync($"/management/products/{initialProduct.Id}/make-not-for-sale", null);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await using (var dbContext = CreateDbContext())
        {
            var updatedProduct = await dbContext.Products.SingleAsync();
            updatedProduct.Should().BeEquivalentTo(new
            {
                initialProduct.Id,
                initialProduct.Title,
                initialProduct.Price,
                IsForSale = false
            });
        }
    }

    [Fact]
    public async Task MakeProductNotForSaleTestSuite_WhenProductAlreadyNotForSale_ShouldReturnBadRequest()
    {
        var productForSale = new ProductEntity
        {
            Id = Guid.NewGuid(),
            Title = "Product",
            Price = 9.99,
            IsForSale = false
        };

        await using (var dbContext = CreateDbContext())
        {
            dbContext.Products.Add(productForSale);
            await dbContext.SaveChangesAsync();
        }

        using var httpClient = CreateHttpClient();
        var response = await httpClient.PutAsync($"/management/products/{productForSale.Id}/make-not-for-sale", null);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task MakeProductNotForSaleTestSuite_WhenProductDoesNotExist_ShouldReturnNotFound()
    {
        var notExistingProductId = Guid.NewGuid();

        using var httpClient = CreateHttpClient();
        var response = await httpClient.PutAsync($"/management/products/{notExistingProductId}/make-not-for-sale", null);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

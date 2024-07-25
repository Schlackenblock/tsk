using Tsk.HttpApi.Products;

namespace Tsk.Tests.Products.ForAdmins;

public class MakeProductForSaleTestSuite : TestSuiteBase
{
    [Fact]
    public async Task MakeProductForSale_WhenProductNotForSale_ShouldSucceed()
    {
        var initialProduct = new ProductEntity
        {
            Id = Guid.NewGuid(),
            Title = "Product",
            Price = 9.99,
            IsForSale = false
        };

        await using (var dbContext = CreateDbContext())
        {
            dbContext.Products.Add(initialProduct);
            await dbContext.SaveChangesAsync();
        }

        using var httpClient = CreateHttpClient();
        var response = await httpClient.PutAsync($"/management/products/{initialProduct.Id}/make-for-sale", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await using (var dbContext = CreateDbContext())
        {
            var updatedProduct = await dbContext.Products.SingleAsync();
            updatedProduct.Should().BeEquivalentTo(new
            {
                initialProduct.Id,
                initialProduct.Title,
                initialProduct.Price,
                IsForSale = true
            });
        }
    }

    [Fact]
    public async Task MakeProductForSale_WhenProductAlreadyForSale_ShouldReturnBadRequest()
    {
        var productForSale = new ProductEntity
        {
            Id = Guid.NewGuid(),
            Title = "Product",
            Price = 9.99,
            IsForSale = true
        };

        await using (var dbContext = CreateDbContext())
        {
            dbContext.Products.Add(productForSale);
            await dbContext.SaveChangesAsync();
        }

        using var httpClient = CreateHttpClient();
        var response = await httpClient.PutAsync($"/management/products/{productForSale.Id}/make-for-sale", null);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task MakeProductForSale_WhenProductDoesNotExist_ShouldReturnNotFound()
    {
        var notExistingProductId = Guid.NewGuid();

        using var httpClient = CreateHttpClient();
        var response = await httpClient.PutAsync($"/management/products/{notExistingProductId}/make-for-sale", null);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

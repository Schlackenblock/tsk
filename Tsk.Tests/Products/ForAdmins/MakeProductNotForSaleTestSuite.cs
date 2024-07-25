using Tsk.HttpApi.Products;

namespace Tsk.Tests.Products.ForAdmins;

public class MakeProductNotForSaleTestSuite : TestSuiteBase
{
    [Fact]
    public async Task MakeProductNotForSaleTestSuite_WhenProductForSale_ShouldSucceed()
    {
        var productForSale = new ProductEntity
        {
            Id = Guid.NewGuid(),
            Title = "Product for sale",
            Price = 9.99,
            IsForSale = true
        };
        Context.Products.Add(productForSale);
        await Context.SaveChangesAsync();

        var response = await HttpClient.PutAsync($"/management/products/{productForSale.Id}/make-not-for-sale", null);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        productForSale.IsForSale.Should().Be(false);
    }

    [Fact]
    public async Task MakeProductNotForSaleTestSuite_WhenProductAlreadyNotForSale_ShouldReturnBadRequest()
    {
        var productForSale = new ProductEntity
        {
            Id = Guid.NewGuid(),
            Title = "Product for sale",
            Price = 9.99,
            IsForSale = false
        };
        Context.Products.Add(productForSale);
        await Context.SaveChangesAsync();

        var response = await HttpClient.PutAsync($"/management/products/{productForSale.Id}/make-not-for-sale", null);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task MakeProductNotForSaleTestSuite_WhenProductDoesNotExist_ShouldReturnNotFound()
    {
        var notExistingProductId = Guid.NewGuid();
        var response = await HttpClient.PutAsync($"/management/products/{notExistingProductId}/make-not-for-sale", null);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

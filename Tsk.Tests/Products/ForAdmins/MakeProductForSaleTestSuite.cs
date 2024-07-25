using Tsk.HttpApi.Products;

namespace Tsk.Tests.Products.ForAdmins;

public class MakeProductForSaleTestSuite : TestSuiteBase
{
    [Fact]
    public async Task MakeProductForSale_WhenProductNotForSale_ShouldSucceed()
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

        var response = await HttpClient.PutAsync($"/management/products/{productForSale.Id}/make-for-sale", null);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        productForSale.IsForSale.Should().Be(true);
    }

    [Fact]
    public async Task MakeProductForSale_WhenProductAlreadyForSale_ShouldReturnBadRequest()
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

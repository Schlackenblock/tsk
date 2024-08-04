using Tsk.HttpApi.Entities;

namespace Tsk.Tests.Products.ForAdmins;

public class MakeProductNotForSaleTestSuite : IntegrationTestSuiteBase
{
    [Fact]
    public async Task MakeProductNotForSaleTestSuite_WhenProductForSale_ShouldSucceed()
    {
        var initialProduct = TestDataGenerator.GenerateProduct(config: product => product.IsForSale = true);
        await SeedInitialDataAsync(initialProduct);

        var response = await HttpClient.PutAsync($"/management/products/{initialProduct.Id}/make-not-for-sale", null);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await AssertDbStateAsync(async dbContext =>
        {
            var updatedProduct = await dbContext.Products.SingleAsync();
            updatedProduct.Should().BeEquivalentTo(new Product
            {
                Id = initialProduct.Id,
                Code = initialProduct.Code,
                Title = initialProduct.Title,
                Pictures = initialProduct.Pictures,
                Price = initialProduct.Price,
                IsForSale = false
            });
        });
    }

    [Fact]
    public async Task MakeProductNotForSaleTestSuite_WhenProductAlreadyNotForSale_ShouldReturnBadRequest()
    {
        var productForSale = TestDataGenerator.GenerateProduct(config: product => product.IsForSale = false);
        await SeedInitialDataAsync(productForSale);

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

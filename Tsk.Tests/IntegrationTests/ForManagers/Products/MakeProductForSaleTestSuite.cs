using Tsk.HttpApi.Entities;

namespace Tsk.Tests.IntegrationTests.ForManagers.Products;

public class MakeProductForSaleTestSuite : IntegrationTestSuiteBase
{
    [Fact]
    public async Task MakeProductForSale_WhenProductNotForSale_ShouldSucceed()
    {
        var initialProduct = TestDataGenerator.GenerateProduct(isForSale: false);
        await SeedInitialDataAsync(initialProduct);

        var response = await HttpClient.PutAsync($"/management/products/{initialProduct.Id}/make-for-sale", null);
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
                IsForSale = true
            });
        });
    }

    [Fact]
    public async Task MakeProductForSale_WhenProductAlreadyForSale_ShouldReturnBadRequest()
    {
        var productForSale = TestDataGenerator.GenerateProduct(isForSale: true);
        await SeedInitialDataAsync(productForSale);

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

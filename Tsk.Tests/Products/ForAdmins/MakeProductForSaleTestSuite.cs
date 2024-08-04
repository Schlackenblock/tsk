using Tsk.HttpApi.Entities;

namespace Tsk.Tests.Products.ForAdmins;

public class MakeProductForSaleTestSuite : IntegrationTestSuiteBase
{
    [Fact]
    public async Task MakeProductForSale_WhenProductNotForSale_ShouldSucceed()
    {
        var initialProduct = new Product
        {
            Id = Guid.NewGuid(),
            Code = "P",
            Title = "Product",
            Pictures = ["Picture 1", "Picture 2"],
            Price = 9.99m,
            IsForSale = false
        };
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
        var productForSale = new Product
        {
            Id = Guid.NewGuid(),
            Code = "P",
            Title = "Product",
            Pictures = ["Picture 1", "Picture 2"],
            Price = 9.99m,
            IsForSale = true
        };
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

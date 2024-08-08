namespace Tsk.Tests.IntegrationTests.ForAdmins.Products;

public class DeleteProductTestSuite : IntegrationTestSuiteBase
{
    [Fact]
    public async Task DeleteProduct_WhenProductIsForSale_ShouldSucceed()
    {
        var product = TestDataGenerator.GenerateProduct(isForSale: true);
        await SeedInitialDataAsync(product);

        var response = await HttpClient.DeleteAsync($"/management/products/{product.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await AssertDbStateAsync(async dbContext =>
        {
            var productsExist = await dbContext.Products.AnyAsync();
            productsExist.Should().BeFalse();
        });
    }
    [Fact]
    public async Task DeleteProduct_WhenProductIsNotForSale_ShouldSucceed()
    {
        var product = TestDataGenerator.GenerateProduct(isForSale: false);
        await SeedInitialDataAsync(product);

        var response = await HttpClient.DeleteAsync($"/management/products/{product.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await AssertDbStateAsync(async dbContext =>
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

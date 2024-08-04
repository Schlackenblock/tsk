using Tsk.HttpApi.Products.ForAdmins;

namespace Tsk.Tests.Products.ForAdmins;

public class GetProductsTestSuite : IntegrationTestSuiteBase
{
    [Fact]
    public async Task GetProducts_WhenManyExist_ShouldReturnBothForSaleAndNotForSale()
    {
        var existingProducts = new[]
        {
            TestDataGenerator.GenerateProduct(index: 1, isForSale: true),
            TestDataGenerator.GenerateProduct(index: 2, isForSale: false)
        };
        await SeedInitialDataAsync(existingProducts);

        var response = await HttpClient.GetAsync("/management/products");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productDtos = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        productDtos.Should().BeEquivalentTo(existingProducts);
    }

    [Fact]
    public async Task GetProducts_WhenNoneExist_ShouldReturnNone()
    {
        var response = await HttpClient.GetAsync("/management/products");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productDtos = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        productDtos.Should().BeEmpty();
    }
}

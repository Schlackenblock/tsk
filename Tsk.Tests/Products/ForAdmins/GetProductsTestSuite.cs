using Tsk.HttpApi.Products.ForAdmins;

namespace Tsk.Tests.Products.ForAdmins;

public class GetProductsTestSuite : IntegrationTestSuiteBase
{
    [Fact]
    public async Task GetProducts_WhenManyExist_ShouldReturnBothForSaleAndNotForSale()
    {
        var existingProducts = new[]
        {
            TestDataGenerator.GenerateProduct(index: 1, config: product => product.IsForSale = true),
            TestDataGenerator.GenerateProduct(index: 2, config: product => product.IsForSale = false)
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

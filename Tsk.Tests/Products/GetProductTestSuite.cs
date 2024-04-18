using Tsk.HttpApi.Products;

namespace Tsk.Tests.Products;

public class GetProductTestSuite : TestSuiteBase
{
    [Fact]
    public async Task GetProduct_WhenProductExists_ShouldSucceed()
    {
        var existingProduct = ProductTestData.GenerateProduct();
        Context.Products.Add(existingProduct);
        await Context.SaveChangesAsync();

        var response = await HttpClient.GetAsync($"/products/{existingProduct.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productDto = await response.Content.ReadFromJsonAsync<ProductDto>();
        productDto.Should().BeEquivalentTo(existingProduct);
    }

    [Fact]
    public async Task GetProduct_WhenProductDoesNotExist_ShouldFail()
    {
        var notExistingProductId = Guid.NewGuid();
        var response = await HttpClient.GetAsync($"/products/{notExistingProductId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

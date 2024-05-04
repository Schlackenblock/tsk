using Tsk.HttpApi.Products;

namespace Tsk.Tests.Products;

public class GetSingleProductTestSuite : TestSuiteBase
{
    [Fact]
    public async Task GetProduct_WhenExists_ShouldSucceed()
    {
        var existingProduct = ProductTestData.GenerateProduct();
        Context.Products.Add(existingProduct);
        await Context.SaveChangesAsync();

        var response = await HttpClient.GetAsync($"/products/{existingProduct.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var returnedProduct = await response.Content.ReadFromJsonAsync<ProductDto>();
        returnedProduct.Should().BeEquivalentTo(existingProduct);
    }

    [Fact]
    public async Task GetProduct_WhenDoesNotExist_ShouldFail()
    {
        var notExistingProductId = Guid.NewGuid();
        var response = await HttpClient.GetAsync($"/products/{notExistingProductId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

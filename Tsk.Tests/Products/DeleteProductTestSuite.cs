using Tsk.HttpApi.Products;

namespace Tsk.Tests.Products;

public class DeleteProductTestSuite : TestSuiteBase
{
    [Fact]
    public async Task DeleteProduct_WhenProductExists_ShouldSucceed()
    {
        var existingProduct = ProductTestData.GenerateProduct();
        Context.Products.Add(existingProduct);
        await Context.SaveChangesAsync();

        var response = await HttpClient.DeleteAsync($"/products/{existingProduct.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var deletedProductDto = await response.Content.ReadFromJsonAsync<ProductDto>();
        deletedProductDto.Should().BeEquivalentTo(existingProduct);

        var persistedProducts = await Context.Products.ToListAsync();
        persistedProducts.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteProduct_WhenProductDoesNotExist_ShouldFail()
    {
        var notExistingProductId = Guid.NewGuid();
        var response = await HttpClient.DeleteAsync($"/products/{notExistingProductId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

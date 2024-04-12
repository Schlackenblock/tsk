using Tsk.HttpApi.Products;

namespace Tsk.Tests.Products;

public class UpdateProductTestSuite : TestSuiteBase
{
    [Fact]
    public async Task UpdateProduct_WhenProductExists_ShouldSucceed()
    {
        var existingProduct = ProductTestData.GenerateProduct();
        Context.Products.Add(existingProduct);
        await Context.SaveChangesAsync();

        var updateProductDto = ProductTestData.GenerateUpdateProductDto();

        var response = await HttpClient.PutAsJsonAsync($"/products/{existingProduct.Id}", updateProductDto);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedProductDto = await response.Content.ReadFromJsonAsync<ProductDto>();
        updatedProductDto!.Id.Should().Be(existingProduct.Id);
        updatedProductDto.Should().BeEquivalentTo(updateProductDto);

        existingProduct.Should().BeEquivalentTo(updatedProductDto);
    }

    [Fact]
    public async Task UpdateProduct_WhenProductDoesNotExist_ShouldFail()
    {
        var notExistingProductId = Guid.NewGuid();
        var updateProductDto = ProductTestData.GenerateUpdateProductDto();

        var response = await HttpClient.PutAsJsonAsync($"/products/{notExistingProductId}", updateProductDto);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

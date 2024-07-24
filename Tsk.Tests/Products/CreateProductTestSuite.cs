using Tsk.HttpApi.Products;

namespace Tsk.Tests.Products;

public class CreateProductTestSuite : TestSuiteBase
{
    [Fact]
    public async Task CreateProduct_WhenValid_ShouldSucceed()
    {
        var createProductDto = ProductFaker.MakeCreateDto();

        var response = await HttpClient.PostAsJsonAsync("/products", createProductDto);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var createdProductDto = await response.Content.ReadFromJsonAsync<ProductDto>();
        createdProductDto.Should().BeEquivalentTo(createProductDto);

        var persistedProduct = await Context.Products.SingleAsync();
        persistedProduct.Should().BeEquivalentTo(createdProductDto);
    }
}

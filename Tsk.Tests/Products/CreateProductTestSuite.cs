using Tsk.HttpApi.Products;

namespace Tsk.Tests.Products;

public class CreateProductTestSuite : TestSuiteBase
{
    [Fact]
    public async Task CreateProduct_WhenValid_ShouldSucceed()
    {
        var createProductDto = new CreateProductDto(
            Title: "High Performance Concrete Admixture 20 lbs",
            Description: "20 LB. BAG - High Performance Admixture for Concrete - gray color",
            Price: 47
        );

        var response = await HttpClient.PostAsJsonAsync("/products", createProductDto);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var createdProductDto = await response.Content.ReadFromJsonAsync<ProductDto>();
        createdProductDto.Should().BeEquivalentTo(createProductDto);

        var persistedProduct = await Context.Products.SingleAsync();
        persistedProduct.Should().BeEquivalentTo(createdProductDto);
    }
}

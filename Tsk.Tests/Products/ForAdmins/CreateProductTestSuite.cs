using Tsk.HttpApi.Products.ForAdmins;

namespace Tsk.Tests.Products.ForAdmins;

public class CreateProductTestSuite : TestSuiteBase
{
    [Fact]
    public async Task CreateProduct_WhenValid_ShouldSucceed()
    {
        var createProductDto = new CreateProductDto
        {
            Title = "Product",
            Price = 9.99
        };

        var response = await HttpClient.PostAsJsonAsync("/management/products", createProductDto);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var createdProductDto = await response.Content.ReadFromJsonAsync<ProductDto>();
        createdProductDto.Should().BeEquivalentTo(new
        {
            createProductDto.Title,
            createProductDto.Price,
            IsForSale = false
        });

        var persistedProduct = await Context.Products.SingleAsync();
        persistedProduct.Should().BeEquivalentTo(new
        {
            createdProductDto!.Id,
            createdProductDto.Title,
            createdProductDto.Price,
            createdProductDto.IsForSale
        });
    }
}

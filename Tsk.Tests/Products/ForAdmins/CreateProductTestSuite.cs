using Tsk.HttpApi.Entities;
using Tsk.HttpApi.Products.ForAdmins;

namespace Tsk.Tests.Products.ForAdmins;

public class CreateProductTestSuite : IntegrationTestSuiteBase
{
    [Fact]
    public async Task CreateProduct_WhenValid_ShouldSucceed()
    {
        var createProductDto = new CreateProductDto { Code = "P", Title = "Product", Price = 9.99m };

        var response = await HttpClient.PostAsJsonAsync("/management/products", createProductDto);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var createdProductDto = await response.Content.ReadFromJsonAsync<ProductDto>();
        createdProductDto.Should().BeEquivalentTo(
            new Product
            {
                Id = default,
                Code = createProductDto.Code,
                Title = createProductDto.Title,
                Price = createProductDto.Price,
                IsForSale = false
            },
            config => config.Excluding(product => product.Id)
        );

        await CallDbAsync(async dbContext =>
        {
            var persistedProduct = await dbContext.Products.SingleAsync();
            persistedProduct.Should().BeEquivalentTo(new Product
            {
                Id = createdProductDto!.Id,
                Code = createdProductDto.Code,
                Title = createdProductDto.Title,
                Price = createdProductDto.Price,
                IsForSale = createdProductDto.IsForSale
            });
        });
    }
}

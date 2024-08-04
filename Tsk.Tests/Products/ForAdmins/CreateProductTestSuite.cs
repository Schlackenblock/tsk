using Tsk.HttpApi.Entities;
using Tsk.HttpApi.Products.ForAdmins;

namespace Tsk.Tests.Products.ForAdmins;

public class CreateProductTestSuite : IntegrationTestSuiteBase
{
    [Fact]
    public async Task CreateProduct_WhenValid_ShouldSucceed()
    {
        var createProductDto = new CreateProductDto { Code = "P", Title = "Product", Pictures = ["Product Picture"], Price = 9.99m };

        var response = await HttpClient.PostAsJsonAsync("/management/products", createProductDto);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var createdProductDto = await response.Content.ReadFromJsonAsync<ProductDto>();
        createdProductDto.Should().BeEquivalentTo(
            new Product
            {
                Id = default,
                Code = createProductDto.Code,
                Title = createProductDto.Title,
                Pictures = createProductDto.Pictures,
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
                Pictures = createdProductDto.Pictures,
                Price = createdProductDto.Price,
                IsForSale = createdProductDto.IsForSale
            });
        });
    }

    [Fact]
    public async Task CreateProduct_WhenCodeIsAlreadyInUse_ShouldFail()
    {
        var existingProductWithSameCode = new Product
        {
            Id = Guid.NewGuid(),
            Code = "P",
            Title = "Product 1",
            Pictures = ["Product 1 Picture 1", "Product 1 Picture 2"],
            Price = 9.99m,
            IsForSale = false
        };

        await CallDbAsync(async dbContext =>
        {
            dbContext.Products.Add(existingProductWithSameCode);
            await dbContext.SaveChangesAsync();
        });

        var createProductDto = new CreateProductDto { Code = "P", Title = "Product", Pictures = ["Product Picture"], Price = 9.99m };

        var response = await HttpClient.PostAsJsonAsync("/management/products", createProductDto);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}

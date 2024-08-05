using Tsk.HttpApi.Entities;
using Tsk.HttpApi.Products.ForAdmins;

namespace Tsk.Tests.IntegrationTests.Products.ForAdmins;

public class CreateProductTestSuite : IntegrationTestSuiteBase
{
    [Fact]
    public async Task CreateProduct_WithMultiplePictures_ShouldSucceed()
    {
        var createProductDto = TestDataGenerator
            .GenerateProduct(pictures: ["Picture #1", "Picture #2"])
            .ToCreateProductDto();

        var response = await HttpClient.PostAsJsonAsync("/management/products", createProductDto);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var createdProductDto = await response.Content.ReadFromJsonAsync<ProductDto>();
        createdProductDto.Should().BeEquivalentTo(
            new ProductDto
            {
                Id = default,
                Code = createProductDto.Code,
                Title = createProductDto.Title,
                Pictures = createProductDto.Pictures,
                IsForSale = false,
                Price = createProductDto.Price
            },
            config => config.Excluding(product => product.Id)
        );

        await AssertDbStateAsync(async dbContext =>
        {
            var createdProduct = await dbContext.Products.SingleAsync();
            createdProduct.Should().BeEquivalentTo(new Product
            {
                Id = createdProductDto!.Id,
                Code = createdProductDto.Code,
                Title = createdProductDto.Title,
                Pictures = createdProductDto.Pictures,
                IsForSale = false,
                Price = createdProductDto.Price
            });
        });
    }

    [Fact]
    public async Task CreateProduct_WithSinglePicture_ShouldSucceed()
    {
        var createProductDto = TestDataGenerator.GenerateProduct(pictures: ["Picture"]).ToCreateProductDto();

        var response = await HttpClient.PostAsJsonAsync("/management/products", createProductDto);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var createdProductDto = await response.Content.ReadFromJsonAsync<ProductDto>();
        createdProductDto.Should().BeEquivalentTo(
            new ProductDto
            {
                Id = default,
                Code = createProductDto.Code,
                Title = createProductDto.Title,
                Pictures = createProductDto.Pictures,
                IsForSale = false,
                Price = createProductDto.Price
            },
            config => config.Excluding(product => product.Id)
        );

        await AssertDbStateAsync(async dbContext =>
        {
            var createdProduct = await dbContext.Products.SingleAsync();
            createdProduct.Should().BeEquivalentTo(new Product
            {
                Id = createdProductDto!.Id,
                Code = createdProductDto.Code,
                Title = createdProductDto.Title,
                Pictures = createdProductDto.Pictures,
                IsForSale = false,
                Price = createdProductDto.Price
            });
        });
    }

    [Fact]
    public async Task CreateProduct_WithoutPictures_ShouldSucceed()
    {
        var createProductDto = TestDataGenerator.GenerateProduct(pictures: []).ToCreateProductDto();

        var response = await HttpClient.PostAsJsonAsync("/management/products", createProductDto);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var createdProductDto = await response.Content.ReadFromJsonAsync<ProductDto>();
        createdProductDto.Should().BeEquivalentTo(
            new ProductDto
            {
                Id = default,
                Code = createProductDto.Code,
                Title = createProductDto.Title,
                Pictures = createProductDto.Pictures,
                IsForSale = false,
                Price = createProductDto.Price
            },
            config => config.Excluding(product => product.Id)
        );

        await AssertDbStateAsync(async dbContext =>
        {
            var createdProduct = await dbContext.Products.SingleAsync();
            createdProduct.Should().BeEquivalentTo(new Product
            {
                Id = createdProductDto!.Id,
                Code = createdProductDto.Code,
                Title = createdProductDto.Title,
                Pictures = createdProductDto.Pictures,
                IsForSale = false,
                Price = createdProductDto.Price
            });
        });
    }

    [Fact]
    public async Task CreateProduct_WhenCodeIsAlreadyInUse_ShouldFail()
    {
        var existingProductWithSameCode = TestDataGenerator.GenerateProduct(code: "Same Code");
        await SeedInitialDataAsync(existingProductWithSameCode);

        var createProductDto = TestDataGenerator.GenerateProduct(code: "Same Code").ToCreateProductDto();

        var response = await HttpClient.PostAsJsonAsync("/management/products", createProductDto);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
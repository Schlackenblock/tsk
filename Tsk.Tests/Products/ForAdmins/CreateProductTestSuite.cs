using Tsk.HttpApi.Entities;
using Tsk.HttpApi.Products.ForAdmins;

namespace Tsk.Tests.Products.ForAdmins;

public class CreateProductTestSuite : IntegrationTestSuiteBase
{
    [Fact]
    public async Task CreateProduct_WithMultiplePictures_ShouldSucceed()
    {
        var createProductDto = new CreateProductDto { Code = "P", Title = "Product", Pictures = ["Picture #1", "Picture #2"], Price = 9.99m };

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

        await CallDbAsync(async dbContext =>
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
        var createProductDto = new CreateProductDto { Code = "P", Title = "Product", Pictures = ["Picture"], Price = 9.99m };

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

        await CallDbAsync(async dbContext =>
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
        var createProductDto = new CreateProductDto { Code = "P", Title = "Product", Pictures = [], Price = 9.99m };

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

        await CallDbAsync(async dbContext =>
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

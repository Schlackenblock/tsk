using Tsk.HttpApi.Entities;
using Tsk.HttpApi.Products.ForAdmins;

namespace Tsk.Tests.Products.ForAdmins;

public class UpdateProductTestSuite : IntegrationTestSuiteBase
{
    [Fact]
    public async Task UpdateProduct_WhenProductDidntHavePictures_ShouldBeOverridden()
    {
        var productWithoutPictures = TestDataGenerator.GenerateProduct(pictures: []);
        await SeedInitialDataAsync(productWithoutPictures);

        var updateProductDto = new UpdateProductDto
        {
            Code = "Updated P",
            Title = "Updated Product",
            Pictures = ["Updated picture"],
            Price = 4.99m
        };

        var response = await HttpClient.PutAsJsonAsync($"management/products/{productWithoutPictures.Id}", updateProductDto);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedProductDto = await response.Content.ReadFromJsonAsync<ProductDto>();
        updatedProductDto.Should().BeEquivalentTo(new ProductDto
        {
            Id = productWithoutPictures.Id,
            Code = updateProductDto.Code,
            Title = updateProductDto.Title,
            Pictures = updateProductDto.Pictures,
            IsForSale = productWithoutPictures.IsForSale,
            Price = updateProductDto.Price
        });

        await AssertDbStateAsync(async dbContext =>
        {
            var updatedProduct = await dbContext.Products.SingleAsync();
            updatedProduct.Should().BeEquivalentTo(new Product
            {
                Id = updatedProductDto!.Id,
                Code = updatedProductDto.Code,
                Title = updatedProductDto.Title,
                Pictures = updatedProductDto.Pictures,
                IsForSale = updatedProductDto.IsForSale,
                Price = updatedProductDto.Price
            });
        });
    }

    [Fact]
    public async Task UpdateProduct_WhenProductHadPictures_ShouldBeOverridden()
    {
        var productWithPictures = TestDataGenerator.GenerateProduct(pictures: ["Picture #1", "Picture #2"]);
        await SeedInitialDataAsync(productWithPictures);

        var updateProductDto = new UpdateProductDto
        {
            Code = "Updated P",
            Title = "Updated Product",
            Pictures = ["Updated picture"],
            Price = 4.99m
        };

        var response = await HttpClient.PutAsJsonAsync($"management/products/{productWithPictures.Id}", updateProductDto);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedProductDto = await response.Content.ReadFromJsonAsync<ProductDto>();
        updatedProductDto.Should().BeEquivalentTo(new ProductDto
        {
            Id = productWithPictures.Id,
            Code = updateProductDto.Code,
            Title = updateProductDto.Title,
            Pictures = updateProductDto.Pictures,
            IsForSale = productWithPictures.IsForSale,
            Price = updateProductDto.Price
        });

        await AssertDbStateAsync(async dbContext =>
        {
            var updatedProduct = await dbContext.Products.SingleAsync();
            updatedProduct.Should().BeEquivalentTo(new Product
            {
                Id = updatedProductDto!.Id,
                Code = updatedProductDto.Code,
                Title = updatedProductDto.Title,
                Pictures = updatedProductDto.Pictures,
                IsForSale = updatedProductDto.IsForSale,
                Price = updatedProductDto.Price
            });
        });
    }

    [Fact]
    public async Task UpdateProduct_WhenProductForSale_ShouldStayForSale()
    {
        var initialProduct = TestDataGenerator.GenerateProduct(isForSale: true);
        await SeedInitialDataAsync(initialProduct);

        var updateProductDto = new UpdateProductDto
        {
            Code = "Updated P",
            Title = "Updated product for sale",
            Pictures = ["Updated Picture 1", "Updated Picture 2"],
            Price = 8.99m
        };

        var response = await HttpClient.PutAsJsonAsync($"/management/products/{initialProduct.Id}", updateProductDto);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedProductDto = await response.Content.ReadFromJsonAsync<ProductDto>();
        updatedProductDto.Should().BeEquivalentTo(new Product
        {
            Id = initialProduct.Id,
            Code = updateProductDto.Code,
            Title = updateProductDto.Title,
            Pictures = updateProductDto.Pictures,
            Price = updateProductDto.Price,
            IsForSale = initialProduct.IsForSale
        });

        await AssertDbStateAsync(async dbContext =>
        {
            var updatedProduct = await dbContext.Products.SingleAsync();
            updatedProduct.Should().BeEquivalentTo(updatedProductDto);
        });
    }

    [Fact]
    public async Task UpdateProduct_WhenProductNotForSale_ShouldStayNotForSale()
    {
        var initialProduct = TestDataGenerator.GenerateProduct(isForSale: false);
        await SeedInitialDataAsync(initialProduct);

        var updateProductDto = new UpdateProductDto
        {
            Code = "Updated P",
            Title = "Updated product not for sale",
            Pictures = ["Updated Picture 1", "Updated Picture 2"],
            Price = 8.99m
        };

        var response = await HttpClient.PutAsJsonAsync($"/management/products/{initialProduct.Id}", updateProductDto);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedProductDto = await response.Content.ReadFromJsonAsync<ProductDto>();
        updatedProductDto.Should().BeEquivalentTo(new Product
        {
            Id = initialProduct.Id,
            Code = updateProductDto.Code,
            Title = updateProductDto.Title,
            Pictures = updateProductDto.Pictures,
            Price = updateProductDto.Price,
            IsForSale = initialProduct.IsForSale
        });

        await AssertDbStateAsync(async dbContext =>
        {
            var updatedProduct = await dbContext.Products.SingleAsync();
            updatedProduct.Should().BeEquivalentTo(updatedProductDto);
        });
    }
    [Fact]
    public async Task UpdateProduct_WhenCodeIsAlreadyInUse_ShouldFail()
    {
        var initialProduct = TestDataGenerator.GenerateProduct();
        await SeedInitialDataAsync(initialProduct);

        var anotherProductWithSpecifiedCode = TestDataGenerator.GenerateProduct(code: "Same Code");
        await SeedInitialDataAsync(anotherProductWithSpecifiedCode);

        var updateProductDto = new UpdateProductDto
        {
            Code = "Same Code",
            Title = "Updated product for sale",
            Pictures = ["Updated Picture 1", "Updated Picture 2"],
            Price = 8.99m
        };

        var response = await HttpClient.PutAsJsonAsync($"/management/products/{initialProduct.Id}", updateProductDto);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateProduct_WhenProductDoesNotExist_ShouldFail()
    {
        var notExistingProductId = Guid.NewGuid();

        var updateProductDto = new UpdateProductDto
        {
            Code = "P",
            Title = "Updated not existing product",
            Pictures = ["Updated Picture 1", "Updated Picture 2"],
            Price = 8.99m
        };

        var response = await HttpClient.PutAsJsonAsync($"/management/products/{notExistingProductId}", updateProductDto);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

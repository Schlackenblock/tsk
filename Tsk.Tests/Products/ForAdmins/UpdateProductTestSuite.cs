using Tsk.HttpApi.Products;
using Tsk.HttpApi.Products.ForAdmins;

namespace Tsk.Tests.Products.ForAdmins;

public class UpdateProductTestSuite : TestSuiteBase
{
    [Fact]
    public async Task UpdateProduct_WhenProductForSale_ShouldStayForSale()
    {
        var initialProduct = new ProductEntity
        {
            Id = Guid.NewGuid(),
            Title = "Product for sale",
            Price = 9.99m,
            IsForSale = true
        };

        await CallDbAsync(async dbContext =>
        {
            dbContext.Products.Add(initialProduct);
            await dbContext.SaveChangesAsync();
        });

        var updateProductDto = new UpdateProductDto
        {
            Title = "Updated product for sale",
            Price = 8.99m
        };

        var response = await HttpClient.PutAsJsonAsync($"/management/products/{initialProduct.Id}", updateProductDto);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedProductDto = await response.Content.ReadFromJsonAsync<ProductDto>();
        updatedProductDto.Should().BeEquivalentTo(new
        {
            initialProduct.Id,
            updateProductDto.Title,
            updateProductDto.Price,
            IsForSale = true
        });

        await CallDbAsync(async dbContext =>
        {
            var updatedProduct = await dbContext.Products.SingleAsync();
            updatedProduct.Should().BeEquivalentTo(updatedProductDto);
        });
    }

    [Fact]
    public async Task UpdateProduct_WhenProductNotForSale_ShouldStayForSale()
    {
        var initialProduct = new ProductEntity
        {
            Id = Guid.NewGuid(),
            Title = "Product for sale",
            Price = 9.99m,
            IsForSale = false
        };

        await CallDbAsync(async dbContext =>
        {
            dbContext.Products.Add(initialProduct);
            await dbContext.SaveChangesAsync();
        });

        var updateProductDto = new UpdateProductDto
        {
            Title = "Updated product for sale",
            Price = 8.99m
        };

        var response = await HttpClient.PutAsJsonAsync($"/management/products/{initialProduct.Id}", updateProductDto);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedProductDto = await response.Content.ReadFromJsonAsync<ProductDto>();
        updatedProductDto.Should().BeEquivalentTo(new
        {
            initialProduct.Id,
            updateProductDto.Title,
            updateProductDto.Price,
            IsForSale = false
        });

        await CallDbAsync(async dbContext =>
        {
            var updatedProduct = await dbContext.Products.SingleAsync();
            updatedProduct.Should().BeEquivalentTo(updatedProductDto);
        });

    }

    [Fact]
    public async Task UpdateProduct_WhenProductDoesNotExist_ShouldFail()
    {
        var notExistingProductId = Guid.NewGuid();
        var updateProductDto = new UpdateProductDto
        {
            Title = "Updated not existing product",
            Price = 8.99m
        };

        var response = await HttpClient.PutAsJsonAsync($"/management/products/{notExistingProductId}", updateProductDto);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

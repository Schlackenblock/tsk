using Tsk.HttpApi.Products;
using Tsk.HttpApi.Products.ForAdmins;

namespace Tsk.Tests.Products.ForAdmins;

public class UpdateProductTestSuite : TestSuiteBase
{
    [Fact]
    public async Task UpdateProduct_WhenProductForSale_ShouldStayForSale()
    {
        var productForSale = new ProductEntity
        {
            Id = Guid.NewGuid(),
            Title = "Product for sale",
            Price = 9.99,
            IsForSale = true
        };
        Context.Products.Add(productForSale);
        await Context.SaveChangesAsync();

        var updateProductDto = new UpdateProductDto
        {
            Title = "Updated product for sale",
            Price = 8.99
        };

        var response = await HttpClient.PutAsJsonAsync($"/management/products/{productForSale.Id}", updateProductDto);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedProductDto = await response.Content.ReadFromJsonAsync<ProductDto>();
        updatedProductDto.Should().BeEquivalentTo(new
        {
            productForSale.Id,
            updateProductDto.Title,
            updateProductDto.Price,
            IsForSale = true
        });

        productForSale.Should().BeEquivalentTo(updatedProductDto);
    }

    [Fact]
    public async Task UpdateProduct_WhenProductNotForSale_ShouldStayForSale()
    {
        var productForSale = new ProductEntity
        {
            Id = Guid.NewGuid(),
            Title = "Product for sale",
            Price = 9.99,
            IsForSale = false
        };
        Context.Products.Add(productForSale);
        await Context.SaveChangesAsync();

        var updateProductDto = new UpdateProductDto
        {
            Title = "Updated product for sale",
            Price = 8.99
        };

        var response = await HttpClient.PutAsJsonAsync($"/management/products/{productForSale.Id}", updateProductDto);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedProductDto = await response.Content.ReadFromJsonAsync<ProductDto>();
        updatedProductDto.Should().BeEquivalentTo(new
        {
            productForSale.Id,
            updateProductDto.Title,
            updateProductDto.Price,
            IsForSale = false
        });

        productForSale.Should().BeEquivalentTo(updatedProductDto);
    }

    [Fact]
    public async Task UpdateProduct_WhenProductDoesNotExist_ShouldFail()
    {
        var notExistingProductId = Guid.NewGuid();
        var updateProductDto = new UpdateProductDto
        {
            Title = "Updated not existing product",
            Price = 8.99
        };

        var response = await HttpClient.PutAsJsonAsync($"/management/products/{notExistingProductId}", updateProductDto);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

using Tsk.HttpApi.Products;

namespace Tsk.Tests.Products;

public class UpdateProductTestSuite : TestSuiteBase
{
    [Fact]
    public async Task UpdateProduct_WhenProductExists_ShouldSucceed()
    {
        var productId = Guid.NewGuid();
        var existingProduct = new ProductEntity
        {
            Id = productId,
            Title = "High Performance Concrete Admixture 20 lbs",
            Description = "20 LB. BAG - High Performance Admixture for Concrete - gray color",
            Price = 47
        };
        Context.Products.Add(existingProduct);
        await Context.SaveChangesAsync();

        var updateProductDto = new UpdateProductDto(
            Title: "High Performance Concrete Admixture 10 lbs",
            Description: "10 LB. BAG - High Performance Admixture for Concrete - gray color",
            Price: 28
        );

        var response = await HttpClient.PutAsJsonAsync($"/products/{productId}", updateProductDto);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedProductDto = await response.Content.ReadFromJsonAsync<ProductDto>();
        updatedProductDto!.Id.Should().Be(productId);
        updatedProductDto.Should().BeEquivalentTo(updateProductDto);

        existingProduct.Should().BeEquivalentTo(updatedProductDto);
    }

    [Fact]
    public async Task UpdateProduct_WhenProductDoesNotExist_ShouldFail()
    {
        var notExistingProductId = Guid.NewGuid();
        var updateProductDto = new UpdateProductDto(
            Title: "High Performance Concrete Admixture 10 lbs",
            Description: "10 LB. BAG - High Performance Admixture for Concrete - gray color",
            Price: 28
        );

        var response = await HttpClient.PutAsJsonAsync($"/products/{notExistingProductId}", updateProductDto);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

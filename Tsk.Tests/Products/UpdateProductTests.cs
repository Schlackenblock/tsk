using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Tsk.HttpApi;
using Tsk.HttpApi.Products;
using Xunit;

namespace Tsk.Tests.Products;

public class UpdateProductTests
{
    private readonly HttpClient httpClient;
    private readonly TskContext context;

    public UpdateProductTests(HttpClient httpClient, TskContext context)
    {
        this.httpClient = httpClient;
        this.context = context;
    }

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
        context.Products.Add(existingProduct);
        await context.SaveChangesAsync();

        var updateProductDto = new UpdateProductDto(
            Title: "High Performance Concrete Admixture 10 lbs",
            Description: "10 LB. BAG - High Performance Admixture for Concrete - gray color",
            Price: 28
        );

        var response = await httpClient.PutAsJsonAsync($"/products/{productId}", updateProductDto);
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

        var response = await httpClient.PutAsJsonAsync($"/products/{notExistingProductId}", updateProductDto);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

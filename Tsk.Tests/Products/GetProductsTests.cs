using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Tsk.HttpApi.Products;
using Xunit;

namespace Tsk.Tests.Products;

public class GetProductsTests : TestSuiteBase
{
    [Fact]
    public async Task GetProducts_WhenManyExist_ShouldReturnMany()
    {
        var existingProducts = new List<ProductEntity>
        {
            new ProductEntity
            {
                Id = Guid.NewGuid(),
                Title = "High Performance Concrete Admixture 20 lbs",
                Description = "20 LB. BAG - High Performance Admixture for Concrete - gray color",
                Price = 47
            },
            new ProductEntity
            {
                Id = Guid.NewGuid(),
                Title = "High Performance Concrete Admixture 10 lbs",
                Description = "10 LB. BAG - High Performance Admixture for Concrete - gray color",
                Price = 28
            }
        };
        Context.Products.AddRange(existingProducts);
        await Context.SaveChangesAsync();

        var response = await HttpClient.GetAsync("/products");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productDtos = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        productDtos.Should().BeEquivalentTo(existingProducts);
    }

    [Fact]
    public async Task GetProducts_WhenOneExists_ShouldReturnOne()
    {
        var existingProduct = new ProductEntity
        {
            Id = Guid.NewGuid(),
            Title = "High Performance Concrete Admixture 20 lbs",
            Description = "20 LB. BAG - High Performance Admixture for Concrete - gray color",
            Price = 47
        };
        Context.Products.Add(existingProduct);
        await Context.SaveChangesAsync();

        var response = await HttpClient.GetAsync("/products");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productDtos = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        productDtos!.Single().Should().BeEquivalentTo(existingProduct);
    }

    [Fact]
    public async Task GetProducts_WhenNoneExist_ShouldReturnNone()
    {
        var response = await HttpClient.GetAsync("/products");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productDtos = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        productDtos.Should().BeEmpty();
    }
}

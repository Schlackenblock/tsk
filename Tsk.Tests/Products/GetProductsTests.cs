using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Tsk.HttpApi;
using Tsk.HttpApi.Products;
using Xunit;

namespace Tsk.Tests.Products;

public class GetProductsTests
{
    private readonly HttpClient httpClient;
    private readonly TskContext context;

    public GetProductsTests(HttpClient httpClient, TskContext context)
    {
        this.httpClient = httpClient;
        this.context = context;
    }

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
        context.Products.AddRange(existingProducts);
        await context.SaveChangesAsync();

        var response = await httpClient.GetAsync("/products");
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
        context.Products.Add(existingProduct);
        await context.SaveChangesAsync();

        var response = await httpClient.GetAsync("/products");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productDtos = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        productDtos!.Single().Should().BeEquivalentTo(existingProduct);
    }

    [Fact]
    public async Task GetProducts_WhenNoneExist_ShouldReturnNone()
    {
        var response = await httpClient.GetAsync("/products");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productDtos = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        productDtos.Should().BeEmpty();
    }
}

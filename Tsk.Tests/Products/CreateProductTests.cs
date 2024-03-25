using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tsk.HttpApi;
using Tsk.HttpApi.Products;
using Xunit;

namespace Tsk.Tests.Products;

public class CreateProductTests
{
    private readonly HttpClient httpClient;
    private readonly TskContext context;

    public CreateProductTests(HttpClient httpClient, TskContext context)
    {
        this.httpClient = httpClient;
        this.context = context;
    }

    [Fact]
    public async Task CreateProduct_WhenValid_ShouldSucceed()
    {
        var createProductDto = new CreateProductDto(
            Title: "High Performance Concrete Admixture 20 lbs",
            Description: "20 LB. BAG - High Performance Admixture for Concrete - gray color",
            Price: 47
        );

        var response = await httpClient.PostAsJsonAsync("/products", createProductDto);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var createdProductDto = await response.Content.ReadFromJsonAsync<ProductDto>();
        createdProductDto.Should().BeEquivalentTo(createProductDto);

        var persistedProduct = await context.Products.SingleAsync();
        persistedProduct.Should().BeEquivalentTo(createdProductDto);
    }
}

using Tsk.HttpApi.Entities;
using Tsk.HttpApi.Products.ForAdmins;

namespace Tsk.Tests.Products.ForAdmins;

public class MakeProductsNotForSaleTestSuite : IntegrationTestSuiteBase
{
    [Fact]
    public async Task MakeProductsNotForSale_WhenMultipleProductsSpecified_ShouldSucceed()
    {
        var products = new List<Product>
        {
            new() { Id = Guid.NewGuid(), Code = "P1", Title = "Product #1", IsForSale = true, Price = 1m },
            new() { Id = Guid.NewGuid(), Code = "P2", Title = "Product #2", IsForSale = true, Price = 2m }
        };

        await CallDbAsync(async dbContext =>
        {
            dbContext.Products.AddRange(products);
            await dbContext.SaveChangesAsync();
        });

        var productIds = products.Select(product => product.Id);

        var response = await HttpClient.PutAsJsonAsync("management/products/make-not-for-sale", productIds);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var expectedProductDtos = products.ConvertAll(product => new ProductDto
        {
            Id = product.Id,
            Code = product.Code,
            Title = product.Title,
            IsForSale = false,
            Price = product.Price
        });

        var updatedProductDtos = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        updatedProductDtos.Should().BeEquivalentTo(expectedProductDtos);

        await CallDbAsync(async dbContext =>
        {
            var updatedProducts = await dbContext.Products.ToListAsync();
            updatedProducts.Should().BeEquivalentTo(expectedProductDtos);
        });
    }

    [Fact]
    public async Task MakeProductsNotForSale_WhenSingleProductSpecified_ShouldSucceed()
    {
        var product = new Product { Id = Guid.NewGuid(), Code = "P", Title = "Product", IsForSale = true, Price = 1m };

        await CallDbAsync(async dbContext =>
        {
            dbContext.Products.Add(product);
            await dbContext.SaveChangesAsync();
        });

        var productIds = new[] { product.Id };

        var response = await HttpClient.PutAsJsonAsync("management/products/make-not-for-sale", productIds);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedProductsDto = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        var updatedProductDto = updatedProductsDto!.Single();
        updatedProductDto.Should().BeEquivalentTo(new ProductDto
        {
            Id = product.Id,
            Code = product.Code,
            Title = product.Title,
            IsForSale = false,
            Price = product.Price
        });

        await CallDbAsync(async dbContext =>
        {
            var updatedProduct = await dbContext.Products.SingleAsync();
            updatedProduct.Should().BeEquivalentTo(updatedProductDto);
        });
    }

    [Fact]
    public async Task MakeProductsNotForSale_WhenOneOfSpecifiedProductsIsAlreadyNotForSale_ShouldReturnBadRequest()
    {
        var products = new List<Product>
        {
            new() { Id = Guid.NewGuid(), Code = "P1", Title = "Product #1", IsForSale = true, Price = 1m },
            new() { Id = Guid.NewGuid(), Code = "P2", Title = "Product #2", IsForSale = true, Price = 2m },
            new() { Id = Guid.NewGuid(), Code = "P3", Title = "Product #3", IsForSale = false, Price = 3m }
        };

        await CallDbAsync(async dbContext =>
        {
            dbContext.Products.AddRange(products);
            await dbContext.SaveChangesAsync();
        });

        var productIds = products.Select(product => product.Id);

        var response = await HttpClient.PutAsJsonAsync("management/products/make-not-for-sale", productIds);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        await CallDbAsync(async dbContext =>
        {
            var productsAfterInvocation = await dbContext.Products.ToListAsync();
            productsAfterInvocation.Should().BeEquivalentTo(products);
        });
    }

    [Fact]
    public async Task MakeProductsNotForSale_WhenSingleSpecifiedProductIsAlreadyNotForSale_ShouldReturnBadRequest()
    {
        var product = new Product { Id = Guid.NewGuid(), Code = "P", Title = "Product", IsForSale = false, Price = 1m };

        await CallDbAsync(async dbContext =>
        {
            dbContext.Products.Add(product);
            await dbContext.SaveChangesAsync();
        });

        var productIds = new[] { product.Id };

        var response = await HttpClient.PutAsJsonAsync("management/products/make-not-for-sale", productIds);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        await CallDbAsync(async dbContext =>
        {
            var productAfterInvocation = await dbContext.Products.SingleAsync();
            productAfterInvocation.Should().BeEquivalentTo(product);
        });
    }

    [Fact]
    public async Task MakeProductsNotForSale_WhenOneOfSpecifiedProductsDoesNotExist_ShouldReturnNotFound()
    {
        var existingProducts = new List<Product>
        {
            new() { Id = Guid.NewGuid(), Code = "P1", Title = "Product #1", IsForSale = true, Price = 1m },
            new() { Id = Guid.NewGuid(), Code = "P2", Title = "Product #2", IsForSale = true, Price = 2m }
        };

        await CallDbAsync(async dbContext =>
        {
            dbContext.Products.AddRange(existingProducts);
            await dbContext.SaveChangesAsync();
        });

        var notExistingProductId = Guid.NewGuid();

        var productIds = existingProducts
            .Select(product => product.Id)
            .Append(notExistingProductId);

        var response = await HttpClient.PutAsJsonAsync("management/products/make-not-for-sale", productIds);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        await CallDbAsync(async dbContext =>
        {
            var productsAfterInvocation = await dbContext.Products.ToListAsync();
            productsAfterInvocation.Should().BeEquivalentTo(existingProducts);
        });
    }

    [Fact]
    public async Task MakeProductsNotForSale_WhenSingleSpecifiedProductDoesNotExist_ShouldReturnNotFound()
    {
        var notExistingProductId = Guid.NewGuid();
        var productIds = new[] { notExistingProductId };

        var response = await HttpClient.PutAsJsonAsync("management/products/make-not-for-sale", productIds);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        await CallDbAsync(async dbContext =>
        {
            var productsAfterInvocation = await dbContext.Products.ToListAsync();
            productsAfterInvocation.Should().BeEmpty();
        });
    }

    [Fact]
    public async Task MakeProductsNotForSale_WhenDuplicatingProductIdsProvided_ShouldReturnBadRequest()
    {
        var product = new Product { Id = Guid.NewGuid(), Code = "P", Title = "Product", IsForSale = true, Price = 1m };

        await CallDbAsync(async dbContext =>
        {
            dbContext.Products.Add(product);
            await dbContext.SaveChangesAsync();
        });

        var productIds = Enumerable.Repeat(product.Id, 2);

        var response = await HttpClient.PutAsJsonAsync("management/products/make-not-for-sale", productIds);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        await CallDbAsync(async dbContext =>
        {
            var productAfterInvocation = await dbContext.Products.SingleAsync();
            productAfterInvocation.Should().BeEquivalentTo(product);
        });
    }

    [Fact]
    public async Task MakeProductsNotForSale_WhenNoProductIdsProvided_ShouldReturnBadRequest()
    {
        var productsIds = Enumerable.Empty<Guid>();

        var response = await HttpClient.PutAsJsonAsync("management/products/make-not-for-sale", productsIds);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        await CallDbAsync(async dbContext =>
        {
            var productsAfterInvocation = await dbContext.Products.ToListAsync();
            productsAfterInvocation.Should().BeEmpty();
        });
    }

    [Fact]
    public async Task MakeProductsNotForSale_WhenNullInsteadOfProductIdsProvided_ShouldReturnBadRequest()
    {
        var response = await HttpClient.PutAsJsonAsync<object?>("management/products/make-not-for-sale", null);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        await CallDbAsync(async dbContext =>
        {
            var productsAfterInvocation = await dbContext.Products.ToListAsync();
            productsAfterInvocation.Should().BeEmpty();
        });
    }
}

using Tsk.HttpApi.Entities;
using Tsk.HttpApi.Products.ForCustomers;

namespace Tsk.Tests.Products.ForCustomers;

public class GetProductsTestSuite : IntegrationTestSuiteBase
{
    [Fact]
    public async Task GetProducts_WhenOrderedByPriceAscending_ShouldReturnOrdered()
    {
        var products = new[]
        {
            new Product { Id = Guid.NewGuid(), Title = "Product #1", Price = 2.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Title = "Product #2", Price = 5.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Title = "Product #3", Price = 1.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Title = "Product #4", Price = 4.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Title = "Product #5", Price = 3.99m, IsForSale = true }
        };

        await CallDbAsync(async dbContext =>
        {
            dbContext.Products.AddRange(products);
            await dbContext.SaveChangesAsync();
        });

        var response = await HttpClient.GetAsync("/products?orderBy=price_asc");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var expectedProductDtos = products
            .OrderBy(product => product.Price)
            .Select(product => new ProductDto
            {
                Id = product.Id,
                Title = product.Title,
                Price = product.Price
            });

        var productDtos = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        productDtos.Should().BeEquivalentTo(expectedProductDtos, config => config.WithStrictOrdering());
    }

    [Fact]
    public async Task GetProducts_WhenOrderedByPriceDescending_ShouldReturnOrdered()
    {
        var products = new[]
        {
            new Product { Id = Guid.NewGuid(), Title = "Product #1", Price = 2.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Title = "Product #2", Price = 5.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Title = "Product #3", Price = 1.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Title = "Product #4", Price = 4.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Title = "Product #5", Price = 3.99m, IsForSale = true }
        };

        await CallDbAsync(async dbContext =>
        {
            dbContext.Products.AddRange(products);
            await dbContext.SaveChangesAsync();
        });

        var response = await HttpClient.GetAsync("/products?orderBy=price_desc");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var expectedProductDtos = products
            .OrderByDescending(product => product.Price)
            .Select(product => new ProductDto
            {
                Id = product.Id,
                Title = product.Title,
                Price = product.Price
            });

        var productDtos = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        productDtos.Should().BeEquivalentTo(expectedProductDtos, config => config.WithStrictOrdering());
    }

    [Fact]
    public async Task GetProducts_WhenOrderedByTitleAscending_ShouldReturnOrdered()
    {
        var products = new[]
        {
            new Product { Id = Guid.NewGuid(), Title = "Product #2", Price = 1.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Title = "Product #5", Price = 2.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Title = "Product #1", Price = 3.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Title = "Product #4", Price = 4.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Title = "Product #3", Price = 5.99m, IsForSale = true }
        };

        await CallDbAsync(async dbContext =>
        {
            dbContext.Products.AddRange(products);
            await dbContext.SaveChangesAsync();
        });

        var response = await HttpClient.GetAsync("/products?orderBy=title_asc");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var expectedProductDtos = products
            .OrderBy(product => product.Title)
            .Select(product => new ProductDto
            {
                Id = product.Id,
                Title = product.Title,
                Price = product.Price
            });

        var productDtos = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        productDtos.Should().BeEquivalentTo(expectedProductDtos, config => config.WithStrictOrdering());
    }

    [Fact]
    public async Task GetProducts_WhenOrderedByTitleDescending_ShouldReturnOrdered()
    {
        var products = new[]
        {
            new Product { Id = Guid.NewGuid(), Title = "Product #2", Price = 1.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Title = "Product #5", Price = 2.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Title = "Product #1", Price = 3.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Title = "Product #4", Price = 4.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Title = "Product #3", Price = 5.99m, IsForSale = true }
        };

        await CallDbAsync(async dbContext =>
        {
            dbContext.Products.AddRange(products);
            await dbContext.SaveChangesAsync();
        });

        var response = await HttpClient.GetAsync("/products?orderBy=title_desc");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var expectedProductDtos = products
            .OrderByDescending(product => product.Title)
            .Select(product => new ProductDto
            {
                Id = product.Id,
                Title = product.Title,
                Price = product.Price
            });

        var productDtos = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        productDtos.Should().BeEquivalentTo(expectedProductDtos, config => config.WithStrictOrdering());
    }

    [Fact]
    public async Task GetProducts_WhenOrderedByUnsupportedOption_ShouldRespondWithBadRequest()
    {
        var products = new[]
        {
            new Product { Id = Guid.NewGuid(), Title = "Product #1", Price = 2.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Title = "Product #2", Price = 5.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Title = "Product #3", Price = 1.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Title = "Product #4", Price = 4.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Title = "Product #5", Price = 3.99m, IsForSale = true }
        };

        await CallDbAsync(async dbContext =>
        {
            dbContext.Products.AddRange(products);
            await dbContext.SaveChangesAsync();
        });

        var response = await HttpClient.GetAsync("/products?orderBy=popularity_desc");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetProducts_WhenNotForSaleExist_ShouldReturnOnlyForSale()
    {
        var products = new[]
        {
            new Product { Id = Guid.NewGuid(), Title = "Product #1", Price = 2.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Title = "Product #2", Price = 5.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Title = "Product #3", Price = 1.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Title = "Product #4", Price = 4.99m, IsForSale = false },
            new Product { Id = Guid.NewGuid(), Title = "Product #5", Price = 3.99m, IsForSale = false }
        };

        await CallDbAsync(async dbContext =>
        {
            dbContext.Products.AddRange(products);
            await dbContext.SaveChangesAsync();
        });

        var response = await HttpClient.GetAsync("/products?orderBy=price_asc");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var expectedProductDtos = products
            .Where(product => product.IsForSale)
            .Select(product => new ProductDto
            {
                Id = product.Id,
                Title = product.Title,
                Price = product.Price
            });

        var productDtos = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        productDtos.Should().BeEquivalentTo(expectedProductDtos);
    }

    [Fact]
    public async Task GetProducts_WhenNoneExist_ShouldReturnNone()
    {
        var response = await HttpClient.GetAsync("/products?orderBy=price_asc");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productDtos = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        productDtos.Should().BeEmpty();
    }
}

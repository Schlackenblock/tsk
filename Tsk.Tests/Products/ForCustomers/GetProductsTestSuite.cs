using Tsk.HttpApi.Entities;
using Tsk.HttpApi.Products.ForCustomers;

namespace Tsk.Tests.Products.ForCustomers;

public class GetProductsTestSuite : IntegrationTestSuiteBase
{
    [Fact]
    public async Task GetProducts_WhenFirstPageRequested_ShouldReturnOnlyFirstPage()
    {
        var products = new[]
        {
            new Product { Id = Guid.NewGuid(), Code = "P1", Title = "Product #1", Price = 1.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Code = "P2", Title = "Product #2", Price = 2.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Code = "P3", Title = "Product #3", Price = 3.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Code = "P4", Title = "Product #4", Price = 4.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Code = "P5", Title = "Product #5", Price = 5.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Code = "P6", Title = "Product #6", Price = 6.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Code = "P7", Title = "Product #7", Price = 7.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Code = "P8", Title = "Product #8", Price = 8.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Code = "P9", Title = "Product #9", Price = 9.99m, IsForSale = true }
        };

        await CallDbAsync(async dbContext =>
        {
            dbContext.Products.AddRange(products);
            await dbContext.SaveChangesAsync();
        });

        var response = await HttpClient.GetAsync("/products?orderBy=price_asc&pageNumber=0&pageSize=3");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var expectedProductDtos = products
            .OrderBy(product => product.Price)
            .Take(3)
            .Select(product => new ProductDto
            {
                Id = product.Id,
                Code = product.Code,
                Title = product.Title,
                Price = product.Price
            });

        var productDtos = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        productDtos.Should().BeEquivalentTo(expectedProductDtos, config => config.WithStrictOrdering());
    }

    [Fact]
    public async Task GetProducts_WhenMiddlePageRequested_ShouldReturnOnlyMiddlePage()
    {
        var products = new[]
        {
            new Product { Id = Guid.NewGuid(), Code = "P1", Title = "Product #1", Price = 1.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Code = "P2", Title = "Product #2", Price = 2.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Code = "P3", Title = "Product #3", Price = 3.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Code = "P4", Title = "Product #4", Price = 4.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Code = "P5", Title = "Product #5", Price = 5.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Code = "P6", Title = "Product #6", Price = 6.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Code = "P7", Title = "Product #7", Price = 7.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Code = "P8", Title = "Product #8", Price = 8.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Code = "P9", Title = "Product #9", Price = 9.99m, IsForSale = true }
        };

        await CallDbAsync(async dbContext =>
        {
            dbContext.Products.AddRange(products);
            await dbContext.SaveChangesAsync();
        });

        var response = await HttpClient.GetAsync("/products?orderBy=price_asc&pageNumber=1&pageSize=3");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var expectedProductDtos = products
            .OrderBy(product => product.Price)
            .Skip(3)
            .Take(3)
            .Select(product => new ProductDto
            {
                Id = product.Id,
                Code = product.Code,
                Title = product.Title,
                Price = product.Price
            });

        var productDtos = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        productDtos.Should().BeEquivalentTo(expectedProductDtos, config => config.WithStrictOrdering());
    }

    [Fact]
    public async Task GetProducts_WhenLastPageRequested_ShouldReturnOnlyLastPage()
    {
        var products = new[]
        {
            new Product { Id = Guid.NewGuid(), Code = "P1", Title = "Product #1", Price = 1.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Code = "P2", Title = "Product #2", Price = 2.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Code = "P3", Title = "Product #3", Price = 3.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Code = "P4", Title = "Product #4", Price = 4.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Code = "P5", Title = "Product #5", Price = 5.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Code = "P6", Title = "Product #6", Price = 6.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Code = "P7", Title = "Product #7", Price = 7.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Code = "P8", Title = "Product #8", Price = 8.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Code = "P9", Title = "Product #9", Price = 9.99m, IsForSale = true }
        };

        await CallDbAsync(async dbContext =>
        {
            dbContext.Products.AddRange(products);
            await dbContext.SaveChangesAsync();
        });

        var response = await HttpClient.GetAsync("/products?orderBy=price_asc&pageNumber=2&pageSize=3");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var expectedProductDtos = products
            .OrderBy(product => product.Price)
            .Skip(6)
            .Take(3)
            .Select(product => new ProductDto
            {
                Id = product.Id,
                Code = product.Code,
                Title = product.Title,
                Price = product.Price
            });

        var productDtos = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        productDtos.Should().BeEquivalentTo(expectedProductDtos, config => config.WithStrictOrdering());
    }

    [Fact]
    public async Task GetProducts_WhenNotExistingPageRequested_ShouldReturnEmptyPage()
    {
        var products = new[]
        {
            new Product { Id = Guid.NewGuid(), Code = "P1", Title = "Product #1", Price = 1.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Code = "P2", Title = "Product #2", Price = 2.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Code = "P3", Title = "Product #3", Price = 3.99m, IsForSale = true }
        };

        await CallDbAsync(async dbContext =>
        {
            dbContext.Products.AddRange(products);
            await dbContext.SaveChangesAsync();
        });

        var response = await HttpClient.GetAsync("/products?orderBy=price_asc&pageNumber=1&pageSize=3");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productDtos = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        productDtos.Should().BeEquivalentTo(Enumerable.Empty<ProductDto>());
    }

    [Fact]
    public async Task GetProducts_WhenPartiallyFilledPageRequested_ShouldReturnPartiallyFilledPage()
    {
        var products = new[]
        {
            new Product { Id = Guid.NewGuid(), Code = "P1", Title = "Product #1", Price = 1.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Code = "P2", Title = "Product #2", Price = 2.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Code = "P3", Title = "Product #3", Price = 3.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Code = "P4", Title = "Product #4", Price = 4.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Code = "P5", Title = "Product #5", Price = 5.99m, IsForSale = true }
        };

        await CallDbAsync(async dbContext =>
        {
            dbContext.Products.AddRange(products);
            await dbContext.SaveChangesAsync();
        });

        var response = await HttpClient.GetAsync("/products?orderBy=price_asc&pageNumber=1&pageSize=3");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var expectedProductDtos = products
            .OrderBy(product => product.Price)
            .Skip(3)
            .Take(2)
            .Select(product => new ProductDto
            {
                Id = product.Id,
                Code = product.Code,
                Title = product.Title,
                Price = product.Price
            });

        var productDtos = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        productDtos.Should().BeEquivalentTo(expectedProductDtos, config => config.WithStrictOrdering());
    }

    [Fact]
    public async Task GetProducts_WhenOrderedByPriceAscending_ShouldReturnOrdered()
    {
        var products = new[]
        {
            new Product { Id = Guid.NewGuid(), Code = "P1", Title = "Product #1", Price = 2.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Code = "P2", Title = "Product #2", Price = 5.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Code = "P3", Title = "Product #3", Price = 1.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Code = "P4", Title = "Product #4", Price = 4.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Code = "P5", Title = "Product #5", Price = 3.99m, IsForSale = true }
        };

        await CallDbAsync(async dbContext =>
        {
            dbContext.Products.AddRange(products);
            await dbContext.SaveChangesAsync();
        });

        var response = await HttpClient.GetAsync("/products?orderBy=price_asc&pageNumber=0&pageSize=5");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var expectedProductDtos = products
            .OrderBy(product => product.Price)
            .Select(product => new ProductDto
            {
                Id = product.Id,
                Code = product.Code,
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
            new Product { Id = Guid.NewGuid(), Code = "P1", Title = "Product #1", Price = 2.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Code = "P2", Title = "Product #2", Price = 5.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Code = "P3", Title = "Product #3", Price = 1.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Code = "P4", Title = "Product #4", Price = 4.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Code = "P5", Title = "Product #5", Price = 3.99m, IsForSale = true }
        };

        await CallDbAsync(async dbContext =>
        {
            dbContext.Products.AddRange(products);
            await dbContext.SaveChangesAsync();
        });

        var response = await HttpClient.GetAsync("/products?orderBy=price_desc&pageNumber=0&pageSize=5");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var expectedProductDtos = products
            .OrderByDescending(product => product.Price)
            .Select(product => new ProductDto
            {
                Id = product.Id,
                Code = product.Code,
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
            new Product { Id = Guid.NewGuid(), Code = "P1", Title = "Product #2", Price = 1.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Code = "P2", Title = "Product #5", Price = 2.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Code = "P3", Title = "Product #1", Price = 3.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Code = "P4", Title = "Product #4", Price = 4.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Code = "P5", Title = "Product #3", Price = 5.99m, IsForSale = true }
        };

        await CallDbAsync(async dbContext =>
        {
            dbContext.Products.AddRange(products);
            await dbContext.SaveChangesAsync();
        });

        var response = await HttpClient.GetAsync("/products?orderBy=title_asc&pageNumber=0&pageSize=5");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var expectedProductDtos = products
            .OrderBy(product => product.Title)
            .Select(product => new ProductDto
            {
                Id = product.Id,
                Code = product.Code,
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
            new Product { Id = Guid.NewGuid(), Code = "P1", Title = "Product #2", Price = 1.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Code = "P2", Title = "Product #5", Price = 2.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Code = "P3", Title = "Product #1", Price = 3.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Code = "P4", Title = "Product #4", Price = 4.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Code = "P5", Title = "Product #3", Price = 5.99m, IsForSale = true }
        };

        await CallDbAsync(async dbContext =>
        {
            dbContext.Products.AddRange(products);
            await dbContext.SaveChangesAsync();
        });

        var response = await HttpClient.GetAsync("/products?orderBy=title_desc&pageNumber=0&pageSize=5");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var expectedProductDtos = products
            .OrderByDescending(product => product.Title)
            .Select(product => new ProductDto
            {
                Id = product.Id,
                Code = product.Code,
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
            new Product { Id = Guid.NewGuid(), Code = "P1", Title = "Product #1", Price = 2.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Code = "P2", Title = "Product #2", Price = 5.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Code = "P3", Title = "Product #3", Price = 1.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Code = "P4", Title = "Product #4", Price = 4.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Code = "P5", Title = "Product #5", Price = 3.99m, IsForSale = true }
        };

        await CallDbAsync(async dbContext =>
        {
            dbContext.Products.AddRange(products);
            await dbContext.SaveChangesAsync();
        });

        var response = await HttpClient.GetAsync("/products?orderBy=popularity_desc&pageNumber=0&pageSize=5");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetProducts_WhenNotForSaleExist_ShouldReturnOnlyForSale()
    {
        var products = new[]
        {
            new Product { Id = Guid.NewGuid(), Code = "P1", Title = "Product #1", Price = 2.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Code = "P2", Title = "Product #2", Price = 5.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Code = "P3", Title = "Product #3", Price = 1.99m, IsForSale = true },
            new Product { Id = Guid.NewGuid(), Code = "P4", Title = "Product #4", Price = 4.99m, IsForSale = false },
            new Product { Id = Guid.NewGuid(), Code = "P5", Title = "Product #5", Price = 3.99m, IsForSale = false }
        };

        await CallDbAsync(async dbContext =>
        {
            dbContext.Products.AddRange(products);
            await dbContext.SaveChangesAsync();
        });

        var response = await HttpClient.GetAsync("/products?orderBy=price_asc&pageNumber=0&pageSize=5");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var expectedProductDtos = products
            .Where(product => product.IsForSale)
            .Select(product => new ProductDto
            {
                Id = product.Id,
                Code = product.Code,
                Title = product.Title,
                Price = product.Price
            });

        var productDtos = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        productDtos.Should().BeEquivalentTo(expectedProductDtos);
    }

    [Fact]
    public async Task GetProducts_WhenNoneExist_ShouldReturnNone()
    {
        var response = await HttpClient.GetAsync("/products?orderBy=price_asc&pageNumber=0&pageSize=5");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productDtos = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        productDtos.Should().BeEmpty();
    }
}

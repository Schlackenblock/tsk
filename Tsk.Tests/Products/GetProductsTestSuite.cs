using Tsk.HttpApi.Products;

namespace Tsk.Tests.Products;

public class GetProductsTestSuite : TestSuiteBase
{
    [Fact]
    public async Task GetProducts_WhenOrderingApplied_ShouldReturnOrdered()
    {
        var supportedOrderingOptions = new (string, Func<IEnumerable<ProductEntity>, IEnumerable<ProductEntity>>)[]
        {
            ("TitleAscending", products => products.OrderBy(product => product.Title)),
            ("TitleDescending", products => products.OrderByDescending(product => product.Title)),
            ("PriceAscending", products => products.OrderBy(product => product.Price)),
            ("PriceDescending", products => products.OrderByDescending(product => product.Price))
        };

        foreach (var supportedOrderingOption in supportedOrderingOptions)
        {
            var (orderingOptionName, sortProducts) = supportedOrderingOption;

            var existingProducts = ProductTestData.GenerateProducts(10);
            Context.Products.AddRange(existingProducts);
            await Context.SaveChangesAsync();

            var requestUri = $"/products?OrderingOption={orderingOptionName}&PageNumber=0&PageSize=10";
            var response = await HttpClient.GetAsync(requestUri);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var productsPageDto = await response.Content.ReadFromJsonAsync<ProductsPageDto>();
            productsPageDto!.ProductsCount.Should().Be(existingProducts.Count);
            productsPageDto.PagesCount.Should().Be(1);

            var orderedExistingProducts = sortProducts(existingProducts);
            var productDtos = productsPageDto.Products;
            productDtos.Should().BeEquivalentTo(orderedExistingProducts, options => options.WithStrictOrdering());

            await Context.Products.ExecuteDeleteAsync();
        }
    }

    [Fact]
    public async Task GetProducts_WhenFiltersApplied_ShouldReturnFiltered()
    {
        var minPrice = 10.00;
        var maxPrice = 100.00;
        var existingProducts = new
        {
            ThatWillBeFilteredOutByMinPrice = ProductTestData.GenerateProducts(
                count: 5,
                priceRange: new() { Max = 9.99 }
            ),
            ThatWillBeFilteredOutByMaxPrice = ProductTestData.GenerateProducts(
                count: 5,
                priceRange: new() { Min = 99.99 }
            ),
            ThatWillNotBeFilteredOut = ProductTestData.GenerateProducts(
                count: 15,
                priceRange: new() { Min = minPrice, Max = maxPrice }
            )
        };
        Context.Products.AddRange(existingProducts.ThatWillBeFilteredOutByMinPrice);
        Context.Products.AddRange(existingProducts.ThatWillBeFilteredOutByMaxPrice);
        Context.Products.AddRange(existingProducts.ThatWillNotBeFilteredOut);
        await Context.SaveChangesAsync();

        var pageNumber = 1;
        var pageSize = 10;

        var requestUri =
            "/products?" +
            "OrderingOption=TitleAscending" +
            $"&PageNumber={pageNumber}" +
            $"&PageSize={pageSize}" +
            $"&MinPrice={minPrice}" +
            $"&MaxPrice={maxPrice}";
        var response = await HttpClient.GetAsync(requestUri);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productsPageDto = await response.Content.ReadFromJsonAsync<ProductsPageDto>();

        var expectedProductsCount = existingProducts.ThatWillNotBeFilteredOut.Count;
        productsPageDto!.ProductsCount.Should().Be(expectedProductsCount);

        var expectedPagesCount = (int)(Math.Ceiling((double)expectedProductsCount / pageSize));
        productsPageDto.PagesCount.Should().Be(expectedPagesCount);

        var expectedProducts = existingProducts
            .ThatWillNotBeFilteredOut
            .OrderBy(product => product.Title)
            .Skip(pageNumber * pageSize)
            .Take(pageSize)
            .ToList();
        var returnedProductDtos = productsPageDto.Products;
        returnedProductDtos.Should().BeEquivalentTo(expectedProducts);
    }

    [Fact]
    public async Task GetProducts_WhenPageCountExceeded_ShouldReturnNone()
    {
        var existingProduct = ProductTestData.GenerateProduct();
        Context.Products.Add(existingProduct);
        await Context.SaveChangesAsync();

        var response = await HttpClient.GetAsync("/products?OrderingOption=TitleAscending&PageNumber=1&PageSize=10");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productsPageDto = await response.Content.ReadFromJsonAsync<ProductsPageDto>();
        productsPageDto!.PagesCount.Should().Be(1);
        productsPageDto.ProductsCount.Should().Be(1);
        productsPageDto.Products.Should().BeEmpty();
    }

    [Fact]
    public async Task GetProducts_WhenFilteredOutByMinPrice_ShouldReturnNone()
    {
        var existingProduct = ProductTestData.GenerateProduct();
        Context.Products.Add(existingProduct);
        await Context.SaveChangesAsync();

        var minPrice = existingProduct.Price + 0.01;
        var response = await HttpClient.GetAsync(
            $"/products?OrderingOption=TitleAscending&PageNumber=0&PageSize=10&MinPrice={minPrice}"
        );
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productsPageDto = await response.Content.ReadFromJsonAsync<ProductsPageDto>();
        productsPageDto!.PagesCount.Should().Be(0);
        productsPageDto.ProductsCount.Should().Be(0);
        productsPageDto.Products.Should().BeEmpty();
    }

    [Fact]
    public async Task GetProducts_WhenFilteredOutByMaxPrice_ShouldReturnNone()
    {
        var existingProduct = ProductTestData.GenerateProduct();
        Context.Products.Add(existingProduct);
        await Context.SaveChangesAsync();

        var maxPrice = existingProduct.Price - 0.01;
        var response = await HttpClient.GetAsync(
            $"/products?OrderingOption=TitleAscending&PageNumber=0&PageSize=10&MaxPrice={maxPrice}"
        );
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productsPageDto = await response.Content.ReadFromJsonAsync<ProductsPageDto>();
        productsPageDto!.PagesCount.Should().Be(0);
        productsPageDto.ProductsCount.Should().Be(0);
        productsPageDto.Products.Should().BeEmpty();
    }

    [Fact]
    public async Task GetProducts_WhenNoneExist_ShouldReturnNone()
    {
        var response = await HttpClient.GetAsync("/products?OrderingOption=TitleAscending&PageNumber=0&PageSize=10");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productsPageDto = await response.Content.ReadFromJsonAsync<ProductsPageDto>();
        productsPageDto!.PagesCount.Should().Be(0);
        productsPageDto.ProductsCount.Should().Be(0);
        productsPageDto.Products.Should().BeEmpty();
    }
}

using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tsk.HttpApi.Querying;
using Tsk.HttpApi.Validation;

namespace Tsk.HttpApi.Products.ForCustomers;

[ApiController]
[Route("/products")]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public class ProductController : ControllerBase
{
    private readonly TskDbContext dbContext;

    public ProductController(TskDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    [HttpGet]
    [ProducesResponseType<ProductsPageDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetProducts(
        [Required][FromQuery] ProductOrderingOption orderBy,
        [Required][FromQuery][Range(0, int.MaxValue)] int pageNumber,
        [Required][FromQuery][Range(1, 50)] int pageSize,
        [FromQuery][Price] decimal? minPrice = null,
        [FromQuery][Price] decimal? maxPrice = null)
    {
        var productsQuery = dbContext.Products
            .Where(product => product.IsForSale);

        if (minPrice is not null)
        {
            productsQuery = productsQuery.Where(product => product.Price >= minPrice);
        }
        if (maxPrice is not null)
        {
            productsQuery = productsQuery.Where(product => product.Price <= maxPrice);
        }

        productsQuery = productsQuery.ApplyOrdering(orderBy switch
        {
            ProductOrderingOption.PriceAscending => products => products.OrderBy(product => product.Price),
            ProductOrderingOption.PriceDescending => products => products.OrderByDescending(product => product.Price),
            ProductOrderingOption.TitleAscending => products => products.OrderBy(product => product.Title),
            ProductOrderingOption.TitleDescending => products => products.OrderByDescending(product => product.Title),
            _ => throw new ArgumentOutOfRangeException(nameof(orderBy), orderBy, "Unsupported ordering option.")
        });

        var productsCount = await productsQuery.CountAsync();

        var products = await productsQuery
            .Skip(pageNumber * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var productsPageDto = new ProductsPageDto
        {
            Products = products.ConvertAll(ProductDto.FromProductEntity),
            ProductsCount = productsCount
        };
        return Ok(productsPageDto);
    }
}

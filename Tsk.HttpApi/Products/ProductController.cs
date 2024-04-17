using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tsk.HttpApi.Validation;

namespace Tsk.HttpApi.Products;

[ApiController]
[Route("/products")]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public class ProductController(TskContext context) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<ProductsPageDto>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProducts(
        [FromQuery] [GreaterThan(0)] double? minPrice,
        [FromQuery] [GreaterThan(0)] double? maxPrice,
        [FromQuery] [Required] ProductsOrderingOption orderingOption,
        [FromQuery] [Required] [GreaterThan(0, IsExclusive = false)] int pageNumber,
        [FromQuery] [Required] [Range(1, 25)] int pageSize)
    {
        var filteredProductsQuery = context
            .Products
            .Where(product => minPrice == null || product.Price >= minPrice)
            .Where(product => maxPrice == null || product.Price <= maxPrice);

        var filteredProductsCount = await filteredProductsQuery.CountAsync();
        var pagesCount = (int)Math.Ceiling((double)filteredProductsCount / pageSize);

        var orderedProductsQuery = orderingOption switch
        {
            ProductsOrderingOption.TitleAscending => filteredProductsQuery.OrderBy(product => product.Title),
            ProductsOrderingOption.TitleDescending => filteredProductsQuery.OrderByDescending(product => product.Title),
            ProductsOrderingOption.PriceAscending => filteredProductsQuery.OrderBy(product => product.Price),
            ProductsOrderingOption.PriceDescending => filteredProductsQuery.OrderByDescending(product => product.Price),
            _ => throw new UnreachableException()
        };

        var pagedProducts = await orderedProductsQuery
            .Skip(pageNumber * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var pagedProductDtos = pagedProducts
            .Select(
                product => new ProductDto
                {
                    Id = product.Id,
                    Title = product.Title,
                    Price = product.Price
                }
            )
            .ToList();

        var productsPageDto = new ProductsPageDto
        {
            Products = pagedProductDtos,
            ProductsCount = filteredProductsCount,
            PagesCount = pagesCount
        };

        return Ok(productsPageDto);
    }

    [HttpPost]
    [ProducesResponseType<ProductDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto createProductDto)
    {
        var product = new ProductEntity
        {
            Id = Guid.NewGuid(),
            Title = createProductDto.Title,
            Price = createProductDto.Price
        };

        context.Products.Add(product);
        await context.SaveChangesAsync();

        var productDto = new ProductDto
        {
            Id = product.Id,
            Title = product.Title,
            Price = product.Price
        };
        return Ok(productDto);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType<ProductDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProduct([FromRoute] Guid id, [FromBody] UpdateProductDto updateProductDto)
    {
        var product = await context.Products.SingleOrDefaultAsync(product => product.Id == id);
        if (product is null)
        {
            return NotFound();
        }

        product.Title = updateProductDto.Title;
        product.Price = updateProductDto.Price;
        await context.SaveChangesAsync();

        var productDto = new ProductDto
        {
            Id = product.Id,
            Title = product.Title,
            Price = product.Price
        };
        return Ok(productDto);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType<ProductDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProduct([FromRoute] Guid id)
    {
        var product = await context.Products.SingleOrDefaultAsync(product => product.Id == id);
        if (product is null)
        {
            return NotFound();
        }

        context.Products.Remove(product);
        await context.SaveChangesAsync();

        var productDto = new ProductDto
        {
            Id = product.Id,
            Title = product.Title,
            Price = product.Price
        };
        return Ok(productDto);
    }
}

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
    [HttpGet("{id:guid}")]
    [ProducesResponseType<ProductDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSingleProduct([FromRoute] Guid id)
    {
        var product = await context.Products.SingleOrDefaultAsync(product => product.Id == id);
        if (product is null)
        {
            return new NotFoundResult();
        }

        var productDto = new ProductDto
        {
            Id = product.Id,
            Title = product.Title,
            Price = product.Price
        };
        return Ok(productDto);
    }

    [HttpGet]
    [ProducesResponseType<ProductsPageDto>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProducts(
        [FromQuery][Required] ProductOrderingOption orderBy,
        [FromQuery][Required][Range(1, 100)] int pageSize,
        [FromQuery][Required][GreaterThan(0, IsExclusive = false)] int pageNumber,
        [FromQuery][GreaterThan(0)] double? minPrice,
        [FromQuery][GreaterThan(0)] double? maxPrice,
        [FromQuery] string? prompt)
    {
        if (minPrice > maxPrice)
        {
            ModelState.AddModelError(nameof(minPrice), $"Can't be greater than \"{nameof(maxPrice)}\".");
            return ValidationProblem();
        }

        var filteredProductsQuery = context
            .Products
            .Where(product => minPrice == null || product.Price >= minPrice)
            .Where(product => maxPrice == null || product.Price <= maxPrice)
            .Where(product => prompt == null || EF.Functions.ILike(product.Title, $"%{prompt}%"));
        var productsCount = await filteredProductsQuery.CountAsync();

        var orderedProductsQuery = orderBy switch
        {
            ProductOrderingOption.TitleAscending => filteredProductsQuery.OrderBy(product => product.Title),
            ProductOrderingOption.TitleDescending => filteredProductsQuery.OrderByDescending(product => product.Title),
            ProductOrderingOption.PriceAscending => filteredProductsQuery.OrderBy(product => product.Price),
            ProductOrderingOption.PriceDescending => filteredProductsQuery.OrderByDescending(product => product.Price),
            _ => throw new UnreachableException()
        };
        var paginatedProducts = await orderedProductsQuery
            .Skip(pageNumber * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var productDtos = paginatedProducts.Select(
            product => new ProductDto
            {
                Id = product.Id,
                Title = product.Title,
                Price = product.Price
            }
        );

        var pagesCount = (int)Math.Ceiling((double)productsCount / pageSize);
        var productsPageDto = new ProductsPageDto
        {
            Products = productDtos.ToList(),
            ProductsCount = productsCount,
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

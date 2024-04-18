using System.Diagnostics;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Tsk.HttpApi.Products;

[ApiController]
[Route("/products")]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public class ProductController(TskContext context) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<ProductsPageDto>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProducts([AsParameters] GetProductsDto requestDto)
    {
        var filteredProductsQuery = context
            .Products
            .Where(product => requestDto.MinPrice == null || product.Price >= requestDto.MinPrice)
            .Where(product => requestDto.MaxPrice == null || product.Price <= requestDto.MaxPrice);

        var filteredProductsCount = await filteredProductsQuery.CountAsync();
        var pagesCount = (int)Math.Ceiling((double)filteredProductsCount / requestDto.PageSize);

        var orderedProductsQuery = requestDto.OrderingOption switch
        {
            ProductsOrderingOption.TitleAscending => filteredProductsQuery.OrderBy(product => product.Title),
            ProductsOrderingOption.TitleDescending => filteredProductsQuery.OrderByDescending(product => product.Title),
            ProductsOrderingOption.PriceAscending => filteredProductsQuery.OrderBy(product => product.Price),
            ProductsOrderingOption.PriceDescending => filteredProductsQuery.OrderByDescending(product => product.Price),
            _ => throw new UnreachableException()
        };

        var pagedProducts = await orderedProductsQuery
            .Skip(requestDto.PageNumber * requestDto.PageSize)
            .Take(requestDto.PageSize)
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

using System.ComponentModel.DataAnnotations;
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
    [ProducesResponseType<List<ProductDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProducts([FromQuery][Required] ProductOrderingOption orderBy)
    {
        var productsQuery = orderBy switch
        {
            ProductOrderingOption.TitleAscending => context.Products.OrderBy(product => product.Title),
            ProductOrderingOption.TitleDescending => context.Products.OrderByDescending(product => product.Title),
            ProductOrderingOption.PriceAscending => context.Products.OrderBy(product => product.Price),
            ProductOrderingOption.PriceDescending => context.Products.OrderByDescending(product => product.Price),
            _ => throw new UnreachableException()
        };
        var products = await productsQuery.ToListAsync();

        var productDtos = products.Select(
            product => new ProductDto
            {
                Id = product.Id,
                Title = product.Title,
                Price = product.Price
            }
        );
        return Ok(productDtos);
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

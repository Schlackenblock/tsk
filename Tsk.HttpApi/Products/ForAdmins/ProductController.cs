using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Tsk.HttpApi.Products.ForAdmins;

[ApiController]
[Route("/management/products")]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public class ProductController : ControllerBase
{
    private readonly TskContext context;

    public ProductController(TskContext context)
    {
        this.context = context;
    }

    [HttpGet]
    [ProducesResponseType<List<ProductDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProducts()
    {
        var products = await context.Products.ToListAsync();

        var productDtos = products.Select(
            product => new ProductDto
            {
                Id = product.Id,
                Title = product.Title,
                Price = product.Price,
                IsForSale = product.IsForSale
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
            Price = createProductDto.Price,
            IsForSale = false
        };

        context.Products.Add(product);
        await context.SaveChangesAsync();

        var productDto = new ProductDto
        {
            Id = product.Id,
            Title = product.Title,
            Price = product.Price,
            IsForSale = product.IsForSale
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
            Price = product.Price,
            IsForSale = product.IsForSale
        };
        return Ok(productDto);
    }

    [HttpPut("{id:guid}/make-for-sale")]
    [ProducesResponseType<ProductDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MakeProductForSale([FromRoute] Guid id)
    {
        var product = await context.Products.SingleOrDefaultAsync(product => product.Id == id);
        if (product is null)
        {
            return NotFound();
        }
        if (product.IsForSale)
        {
            return BadRequest("Specified product is already available for sale.");
        }

        product.IsForSale = true;
        await context.SaveChangesAsync();

        var productDto = new ProductDto
        {
            Id = product.Id,
            Title = product.Title,
            Price = product.Price,
            IsForSale = product.IsForSale
        };
        return Ok(productDto);
    }

    [HttpPut("{id:guid}/make-not-for-sale")]
    [ProducesResponseType<ProductDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MakeProductNotForSale([FromRoute] Guid id)
    {
        var product = await context.Products.SingleOrDefaultAsync(product => product.Id == id);
        if (product is null)
        {
            return NotFound();
        }
        if (!product.IsForSale)
        {
            return BadRequest("Specified product is already not available for sale.");
        }

        product.IsForSale = false;
        await context.SaveChangesAsync();

        var productDto = new ProductDto
        {
            Id = product.Id,
            Title = product.Title,
            Price = product.Price,
            IsForSale = product.IsForSale
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

        return Ok();
    }
}
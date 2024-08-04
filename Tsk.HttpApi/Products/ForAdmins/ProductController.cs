using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tsk.HttpApi.Entities;

namespace Tsk.HttpApi.Products.ForAdmins;

[ApiController]
[Route("/management/products")]
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
    [ProducesResponseType<List<ProductDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProducts()
    {
        var products = await dbContext.Products.ToListAsync();
        var productDtos = products.Select(ProductDto.FromProductEntity);
        return Ok(productDtos);
    }

    [HttpPost]
    [ProducesResponseType<ProductDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto createProductDto)
    {
        var codeIsAlreadyInUse = await dbContext.Products
            .Where(product => product.Code == createProductDto.Code)
            .AnyAsync();

        if (codeIsAlreadyInUse)
        {
            return BadRequest("Specified code is already used by some other product.");
        }

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Code = createProductDto.Code,
            Title = createProductDto.Title,
            Pictures = createProductDto.Pictures,
            Price = createProductDto.Price,
            IsForSale = false
        };

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();

        var productDto = ProductDto.FromProductEntity(product);
        return Ok(productDto);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType<ProductDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProduct([FromRoute] Guid id, [FromBody] UpdateProductDto updateProductDto)
    {
        var product = await dbContext.Products.SingleOrDefaultAsync(product => product.Id == id);
        if (product is null)
        {
            return NotFound();
        }

        var codeIsAlreadyInUse = await dbContext.Products
            .Where(anotherProduct => anotherProduct.Id != product.Id)
            .Where(anotherProduct => anotherProduct.Code == updateProductDto.Code)
            .AnyAsync();

        if (codeIsAlreadyInUse)
        {
            return BadRequest("Specified code is already used by some other product.");
        }

        product.Title = updateProductDto.Title;
        product.Price = updateProductDto.Price;
        product.Pictures = updateProductDto.Pictures;
        product.Code = updateProductDto.Code;
        await dbContext.SaveChangesAsync();

        var productDto = ProductDto.FromProductEntity(product);
        return Ok(productDto);
    }

    [HttpPut("{id:guid}/make-for-sale")]
    [ProducesResponseType<ProductDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MakeProductForSale([FromRoute] Guid id)
    {
        var product = await dbContext.Products.SingleOrDefaultAsync(product => product.Id == id);
        if (product is null)
        {
            return NotFound();
        }
        if (product.IsForSale)
        {
            return BadRequest("Specified product is already available for sale.");
        }

        product.IsForSale = true;
        await dbContext.SaveChangesAsync();

        var productDto = ProductDto.FromProductEntity(product);
        return Ok(productDto);
    }

    [HttpPut("make-for-sale")]
    [ProducesResponseType<List<ProductDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MakeProductsForSale(
        [FromBody][Required][MinLength(1)] IReadOnlyCollection<Guid> productIds)
    {
        if (productIds.Distinct().Count() != productIds.Count)
        {
            return BadRequest("Duplicated product IDs were provided.");
        }

        var products = await dbContext.Products
            .Where(product => productIds.Contains(product.Id))
            .ToListAsync();

        if (products.Count < productIds.Count)
        {
            var existingProductIds = products.Select(product => product.Id);
            var missingProductIds = productIds.Except(existingProductIds);

            var serializedProductIds = string.Join(", ", missingProductIds.Select(productId => $"\"{productId}\""));
            return NotFound($"The following products weren't found: {string.Join(", ", serializedProductIds)}.");
        }

        var conflictingProductIds = products
            .Where(product => product.IsForSale)
            .Select(product => product.Id)
            .ToList();
        if (conflictingProductIds.Any())
        {
            var serializedProductIds = string.Join(", ", conflictingProductIds.Select(productId => $"\"{productId}\""));
            return BadRequest($"The following products are already available for sale: {serializedProductIds}.");
        }

        foreach (var product in products)
        {
            product.IsForSale = true;
        }
        await dbContext.SaveChangesAsync();

        var productDtos = products.Select(ProductDto.FromProductEntity);
        return Ok(productDtos);
    }

    [HttpPut("{id:guid}/make-not-for-sale")]
    [ProducesResponseType<ProductDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MakeProductNotForSale([FromRoute] Guid id)
    {
        var product = await dbContext.Products.SingleOrDefaultAsync(product => product.Id == id);
        if (product is null)
        {
            return NotFound();
        }
        if (!product.IsForSale)
        {
            return BadRequest("Specified product is already not available for sale.");
        }

        product.IsForSale = false;
        await dbContext.SaveChangesAsync();

        var productDto = ProductDto.FromProductEntity(product);
        return Ok(productDto);
    }

    [HttpPut("make-not-for-sale")]
    [ProducesResponseType<List<ProductDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MakeProductsNotForSale(
        [FromBody][Required][MinLength(1)] IReadOnlyCollection<Guid> productIds)
    {
        if (productIds.Distinct().Count() != productIds.Count)
        {
            return BadRequest("Duplicated product IDs were provided.");
        }

        var products = await dbContext.Products
            .Where(product => productIds.Contains(product.Id))
            .ToListAsync();

        if (products.Count < productIds.Count)
        {
            var existingProductIds = products.Select(product => product.Id);
            var missingProductIds = productIds.Except(existingProductIds);

            var serializedProductIds = string.Join(", ", missingProductIds.Select(productId => $"\"{productId}\""));
            return NotFound($"The following products weren't found: {string.Join(", ", serializedProductIds)}.");
        }

        var conflictingProductIds = products
            .Where(product => !product.IsForSale)
            .Select(product => product.Id)
            .ToList();
        if (conflictingProductIds.Any())
        {
            var serializedProductIds = string.Join(", ", conflictingProductIds.Select(productId => $"\"{productId}\""));
            return BadRequest($"The following are already not available for sale: {serializedProductIds}.");
        }

        foreach (var product in products)
        {
            product.IsForSale = false;
        }
        await dbContext.SaveChangesAsync();

        var productDtos = products.Select(ProductDto.FromProductEntity);
        return Ok(productDtos);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType<ProductDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProduct([FromRoute] Guid id)
    {
        var product = await dbContext.Products.SingleOrDefaultAsync(product => product.Id == id);
        if (product is null)
        {
            return NotFound();
        }

        dbContext.Products.Remove(product);
        await dbContext.SaveChangesAsync();

        return Ok();
    }
}

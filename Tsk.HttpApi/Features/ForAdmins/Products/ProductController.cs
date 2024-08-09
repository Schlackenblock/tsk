using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tsk.HttpApi.Entities;
using Tsk.HttpApi.Querying;
using Tsk.HttpApi.Validation;

namespace Tsk.HttpApi.Features.ForAdmins.Products;

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
    [ProducesResponseType<ProductsPageDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetProducts(
        [Required][FromQuery] ProductsOrder orderBy,
        [Required][FromQuery][Range(0, int.MaxValue)] int pageNumber,
        [Required][FromQuery][Range(1, 50)] int pageSize,
        [FromQuery] string? search = null,
        [FromQuery][Price] decimal? minPrice = null,
        [FromQuery][Price] decimal? maxPrice = null,
        [FromQuery] bool? isForSale = null)
    {
        var products = await dbContext.Products
            .AsNoTracking()
            .Where(product =>
                search == null ||
                EF.Functions.ILike(product.Title, $"%{search}%") ||
                EF.Functions.ILike(product.Code, search)
            )
            .Where(product => minPrice == null || product.Price >= minPrice)
            .Where(product => maxPrice == null || product.Price <= maxPrice)
            .Where(product => isForSale == null || product.IsForSale == isForSale)
            .ApplyOrdering(orderBy switch
            {
                ProductsOrder.PriceAscending => products => products.OrderBy(product => product.Price),
                ProductsOrder.PriceDescending => products => products.OrderByDescending(product => product.Price),
                ProductsOrder.TitleAscending => products => products.OrderBy(product => product.Title),
                ProductsOrder.TitleDescending => products => products.OrderByDescending(product => product.Title),
                _ => throw new ArgumentOutOfRangeException(nameof(orderBy), orderBy, "Unsupported ordering option.")
            })
            .PaginateAsync(pageNumber, pageSize);

        var productsPageDto = new ProductsPageDto
        {
            Products = products.Items.ConvertAll(ProductDto.FromProductEntity),
            ProductsCount = products.Count
        };
        return Ok(productsPageDto);
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
}

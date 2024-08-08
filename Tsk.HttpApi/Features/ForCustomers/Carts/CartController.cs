using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tsk.HttpApi.Entities;

namespace Tsk.HttpApi.Features.ForCustomers.Carts;

[ApiController]
[Route("/carts")]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public class CartController : ControllerBase
{
    private readonly TskDbContext dbContext;

    public CartController(TskDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    [HttpPost]
    [ProducesResponseType<Guid>(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateCart()
    {
        var cart = new Cart
        {
            Id = Guid.NewGuid(),
            Products = []
        };

        dbContext.Carts.Add(cart);
        await dbContext.SaveChangesAsync();

        return Ok(cart.Id);
    }

    [HttpGet("{cartId:guid}")]
    [ProducesResponseType<CartDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCart([FromRoute] Guid cartId)
    {
        var cart = await dbContext.Carts
            .Where(cart => cart.Id == cartId)
            .SingleOrDefaultAsync();
        if (cart is null)
        {
            return NotFound();
        }

        // TODO: Remove ability to completely delete products.
        var productIds = cart.Products.ConvertAll(cartProduct => cartProduct.ProductId);
        var products = await dbContext.Products
            .Where(product => productIds.Contains(product.Id))
            .ToDictionaryAsync(product => product.Id);
        if (products.Count < productIds.Count)
        {
            var missingProductIds = string.Join(", ", productIds.Except(products.Keys));
            throw new Exception($"Cart \"{cartId}\" references not existing products: \"{missingProductIds}\".");
        }

        var cartDto = new CartDto
        {
            Id = cart.Id,
            Products = cart.Products.ConvertAll(cartProduct =>
            {
                var product = products[cartProduct.ProductId];
                return new CartProductDto
                {
                    Id = cartProduct.ProductId,
                    Title = product.Title,
                    Picture = product.Pictures.FirstOrDefault(),
                    Price = product.Price,
                    Quantity = cartProduct.Quantity
                };
            })
        };
        return Ok(cartDto);
    }

    // TODO: POST   "/carts" - create a cart.
    // TODO: GET    "/carts/{cartId:guid}" - get the cart.
    // TODO: POST   "/carts/{cartId:guid}/product/{productId:guid}/add-to-cart" - add product to the cart.
    // TODO: POST   "/carts/{cartId:guid}/products/{productId:guid}/increase" - increase product quantity.
    // TODO: POST   "/carts/{cartId:guid}/products/{productId:guid}/decrease" - decrease product quantity.
    // TODO: DELETE "/carts/{cartId:guid}" - delete (clear) the cart.
}

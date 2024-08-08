using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;

namespace Tsk.HttpApi.Features.ForCustomers.Carts;

[ApiController]
[Route("/carts")]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public class CartController : ControllerBase
{
    // TODO: POST   "/carts" - create a cart.
    // TODO: GET    "/carts/{cartId:guid}" - get the cart.
    // TODO: POST   "/carts/{cartId:guid}/product/{productId:guid}/add-to-cart" - add product to the cart.
    // TODO: POST   "/carts/{cartId:guid}/products/{productId:guid}/increase" - increase product quantity.
    // TODO: POST   "/carts/{cartId:guid}/products/{productId:guid}/decrease" - decrease product quantity.
    // TODO: DELETE "/carts/{cartId:guid}" - delete (clear) the cart.
}

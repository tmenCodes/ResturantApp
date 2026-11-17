using API.Common;
using Microsoft.AspNetCore.Mvc;

namespace API.Features.Cart.AddCartItem;

/// <summary><c>POST api/Cart/AddItem</c>: adds a menu item to a cart.</summary>
public class AddCartItemEndpoint : IEndpoint
{
    public const string Route = "AddItem";

    public void Map(IEndpointRouteBuilder app) =>
        ApiGroups.Cart(app).MapPost(Route, async (
            [FromBody] OrderItemDto? orderItemDto, AddCartItemHandler handler, CancellationToken cancellationToken) =>
            Results.Ok(await handler.HandleAsync(orderItemDto!, cancellationToken)));
}

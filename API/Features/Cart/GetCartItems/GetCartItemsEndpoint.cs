using API.Common;
using API.Features.Cart.Shared;

namespace API.Features.Cart.GetCartItems;

/// <summary><c>GET api/Order/GetMenuItemByGuid</c>: lists the lines in a cart.</summary>
public class GetCartItemsEndpoint : IEndpoint
{
    public const string Route = "GetMenuItemByGuid";

    public void Map(IEndpointRouteBuilder app) =>
        ApiGroups.Order(app).MapGet(Route, async (GetCartItemsHandler handler, string? guidId = CartIdentity.Default) =>
            Results.Ok(await handler.HandleAsync(guidId)));
}

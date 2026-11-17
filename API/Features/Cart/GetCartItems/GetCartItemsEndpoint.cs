using API.Common;
using API.Features.Cart.Shared;

namespace API.Features.Cart.GetCartItems;

/// <summary><c>GET api/Cart/GetItems</c>: lists the lines in a cart.</summary>
public class GetCartItemsEndpoint : IEndpoint
{
    public const string Route = "GetItems";

    public void Map(IEndpointRouteBuilder app) =>
        ApiGroups.Cart(app).MapGet(Route, async (GetCartItemsHandler handler, string? guidId = CartIdentity.Default) =>
            Results.Ok(await handler.HandleAsync(guidId)));
}

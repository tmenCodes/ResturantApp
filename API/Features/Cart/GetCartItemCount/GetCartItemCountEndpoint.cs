using API.Common;
using API.Features.Cart.Shared;

namespace API.Features.Cart.GetCartItemCount;

/// <summary><c>GET api/Cart/GetItemCount</c>: counts the lines in a cart.</summary>
public class GetCartItemCountEndpoint : IEndpoint
{
    public const string Route = "GetItemCount";

    public void Map(IEndpointRouteBuilder app) =>
        ApiGroups.Cart(app).MapGet(Route, async (GetCartItemCountHandler handler, string? guidId = CartIdentity.Default) =>
            Results.Ok(await handler.HandleAsync(guidId)));
}

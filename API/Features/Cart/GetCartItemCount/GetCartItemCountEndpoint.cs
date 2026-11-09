using API.Common;
using API.Features.Cart.Shared;

namespace API.Features.Cart.GetCartItemCount;

/// <summary><c>GET api/Order/GetMenuItemByGuidSize</c>: counts the lines in a cart.</summary>
public class GetCartItemCountEndpoint : IEndpoint
{
    public const string Route = "GetMenuItemByGuidSize";

    public void Map(IEndpointRouteBuilder app) =>
        ApiGroups.Order(app).MapGet(Route, async (GetCartItemCountHandler handler, string? guidId = CartIdentity.Default) =>
            Results.Ok(await handler.HandleAsync(guidId)));
}

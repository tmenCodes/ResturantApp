using API.Common;

namespace API.Features.Menu.GetMenuItems;

/// <summary><c>GET api/Order/GetMenuItems</c>: lists the menu.</summary>
public class GetMenuItemsEndpoint : IEndpoint
{
    public const string Route = "GetMenuItems";

    public void Map(IEndpointRouteBuilder app) =>
        ApiGroups.Order(app).MapGet(Route, async (GetMenuItemsHandler handler, CancellationToken cancellationToken) =>
            Results.Ok(await handler.HandleAsync(cancellationToken)));
}

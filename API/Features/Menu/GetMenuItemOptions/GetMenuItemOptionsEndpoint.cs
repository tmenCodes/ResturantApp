using API.Common;

namespace API.Features.Menu.GetMenuItemOptions;

/// <summary><c>GET api/Order/GetMenuItemOptions</c>: lists the add-on options.</summary>
public class GetMenuItemOptionsEndpoint : IEndpoint
{
    public const string Route = "GetMenuItemOptions";

    public void Map(IEndpointRouteBuilder app) =>
        ApiGroups.Order(app).MapGet(Route, async (GetMenuItemOptionsHandler handler, CancellationToken cancellationToken) =>
            Results.Ok(await handler.HandleAsync(cancellationToken)));
}

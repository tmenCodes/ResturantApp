using API.Common;

namespace API.Features.Menu.GetImagePaths;

/// <summary><c>GET api/Order/ImagePath</c>: lists the menu's image file names.</summary>
public class GetImagePathsEndpoint : IEndpoint
{
    public const string Route = "ImagePath";

    public void Map(IEndpointRouteBuilder app) =>
        ApiGroups.Order(app).MapGet(Route, async (GetImagePathsHandler handler, CancellationToken cancellationToken) =>
            Results.Ok(await handler.HandleAsync(cancellationToken)));
}

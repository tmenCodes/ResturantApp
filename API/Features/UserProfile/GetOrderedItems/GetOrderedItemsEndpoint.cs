using System.Security.Claims;
using API.Common;

namespace API.Features.UserProfile.GetOrderedItems;

/// <summary><c>GET api/UserInformation/UserHistoryMenu</c>: lists the items the signed-in user has ordered.</summary>
public class GetOrderedItemsEndpoint : IEndpoint
{
    public const string Route = "UserHistoryMenu";

    public void Map(IEndpointRouteBuilder app) =>
        ApiGroups.UserInformation(app).MapGet(Route, async (ClaimsPrincipal user, GetOrderedItemsHandler handler) =>
            Results.Ok(await handler.HandleAsync(user.Identity?.Name ?? string.Empty)))
        .RequireAuthorization();
}

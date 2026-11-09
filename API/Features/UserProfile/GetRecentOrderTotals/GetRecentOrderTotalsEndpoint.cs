using System.Security.Claims;
using API.Common;

namespace API.Features.UserProfile.GetRecentOrderTotals;

/// <summary><c>GET api/UserInformation/UserHistoryPayments</c>: lists the signed-in user's recent order totals.</summary>
public class GetRecentOrderTotalsEndpoint : IEndpoint
{
    public const string Route = "UserHistoryPayments";

    public void Map(IEndpointRouteBuilder app) =>
        ApiGroups.UserInformation(app).MapGet(Route, async (ClaimsPrincipal user, GetRecentOrderTotalsHandler handler) =>
            Results.Ok(await handler.HandleAsync(user.Identity?.Name ?? string.Empty)))
        .RequireAuthorization();
}

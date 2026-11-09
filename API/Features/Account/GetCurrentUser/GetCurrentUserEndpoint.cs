using System.Security.Claims;
using API.Common;
using API.Features.Account.Shared;

namespace API.Features.Account.GetCurrentUser;

/// <summary><c>GET api/RegisterUser/GetTheCurrentUser</c>: the signed-in user's account; 401 when it no longer exists.</summary>
public class GetCurrentUserEndpoint : IEndpoint
{
    public const string Route = "GetTheCurrentUser";

    public void Map(IEndpointRouteBuilder app) =>
        ApiGroups.RegisterUser(app).MapGet(Route, async (
            ClaimsPrincipal user, FindAccountHandler handler, ILogger<GetCurrentUserEndpoint> logger) =>
        {
            logger.LogCritical(user.Identity?.Name);
            return await handler.HandleAsync(user.FindFirstValue(ClaimTypes.NameIdentifier)) is { } account
                ? Results.Ok(account)
                : MvcResults.Problem(StatusCodes.Status401Unauthorized);
        }).RequireAuthorization();
}

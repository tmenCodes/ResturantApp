using System.Security.Claims;
using API.Common;
using API.Features.Account.Shared;

namespace API.Features.Account.IsUserLoggedIn;

/// <summary><c>GET api/RegisterUser/IsUserLoggedIn</c>: true when the signed-in user's account exists; 401 otherwise.</summary>
public class IsUserLoggedInEndpoint : IEndpoint
{
    public const string Route = "IsUserLoggedIn";

    public void Map(IEndpointRouteBuilder app) =>
        ApiGroups.RegisterUser(app).MapGet(Route, async (
            ClaimsPrincipal user, FindAccountHandler handler, ILogger<IsUserLoggedInEndpoint> logger) =>
        {
            logger.LogCritical("--------------------------------------------------------------");
            logger.LogCritical("from the httpcontext user identity");
            logger.LogCritical(user.Identity?.Name);

            var account = await handler.HandleAsync(user.FindFirstValue(ClaimTypes.NameIdentifier));
            if (account is null)
            {
                return MvcResults.Problem(StatusCodes.Status401Unauthorized);
            }

            logger.LogCritical(account.Email);
            return Results.Ok(true);
        }).RequireAuthorization();
}

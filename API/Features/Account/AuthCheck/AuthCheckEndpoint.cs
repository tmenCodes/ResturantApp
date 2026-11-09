using API.Common;

namespace API.Features.Account.AuthCheck;

/// <summary><c>GET api/RegisterUser/user</c>: returns fixed text when the caller is signed in.</summary>
public class AuthCheckEndpoint : IEndpoint
{
    public const string Route = "user";

    private const string SignedInMessage = "youur logged in bro ";

    public void Map(IEndpointRouteBuilder app) =>
        ApiGroups.RegisterUser(app).MapGet(Route, (ILogger<AuthCheckEndpoint> logger) =>
        {
            logger.LogInformation("User is authenticated");
            return MvcResults.Message(StatusCodes.Status200OK, SignedInMessage);
        }).RequireAuthorization();
}

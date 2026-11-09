using API.Common;

namespace API.Features.Account.Logout;

/// <summary>
/// <c>POST api/RegisterUser/SignOut</c>: acknowledges sign-out. JWT is stateless, so nothing is invalidated
/// server-side; the client discards the token.
/// </summary>
public class LogoutEndpoint : IEndpoint
{
    public const string Route = "SignOut";

    private const string SignedOutMessage = "Signed out.";

    public void Map(IEndpointRouteBuilder app) =>
        ApiGroups.RegisterUser(app).MapPost(Route, () => Results.Ok(new { message = SignedOutMessage }));
}

using API.Common;
using Microsoft.AspNetCore.Mvc;

namespace API.Features.Account.Login;

/// <summary><c>POST api/Login/Login</c>: exchanges email and password for a JWT.</summary>
public class LoginEndpoint : IEndpoint
{
    public const string Route = "Login";

    private const string MissingCredentialsMessage = "Email and password are required.";
    private const string LockedOutMessage = "Account is locked. Try again later.";
    private const string InvalidCredentialsMessage = "Invalid email or password.";
    private const string SucceededMessage = "Login successful";

    public void Map(IEndpointRouteBuilder app) =>
        ApiGroups.Login(app).MapPost(Route, async ([FromBody] LoginDTO? login, LoginHandler handler) =>
        {
            if (login?.Email is null || login.Password is null)
            {
                return MvcResults.Message(StatusCodes.Status400BadRequest, MissingCredentialsMessage);
            }

            var result = await handler.HandleAsync(login.Email, login.Password);
            return result.Status switch
            {
                LoginStatus.Succeeded => Results.Ok(new { token = result.Token, message = SucceededMessage }),
                LoginStatus.LockedOut => MvcResults.Message(StatusCodes.Status429TooManyRequests, LockedOutMessage),
                _ => MvcResults.Message(StatusCodes.Status401Unauthorized, InvalidCredentialsMessage)
            };
        });
}

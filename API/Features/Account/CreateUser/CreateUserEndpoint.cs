using API.Common;
using Microsoft.AspNetCore.Mvc;

namespace API.Features.Account.CreateUser;

/// <summary><c>POST api/RegisterUser/CreateUser</c>: registers a user account.</summary>
public class CreateUserEndpoint : IEndpoint
{
    public const string Route = "CreateUser";

    private const string EmailRequiredMessage = "Email is required.";
    private const string FullNameRequiredMessage = "Full name is required.";
    private const string UsernameRequiredMessage = "Username is required.";
    private const string EmailInUseMessage = "Email already in use.";
    private const string CreatedMessage = "User created successfully";

    public void Map(IEndpointRouteBuilder app) =>
        ApiGroups.RegisterUser(app).MapPost(Route, async ([FromBody] WebUserDto? userDTO, CreateUserHandler handler) =>
            MissingFieldMessage(userDTO!) is { } missing
                ? MvcResults.Message(StatusCodes.Status400BadRequest, missing)
                : ToResult(await handler.HandleAsync(userDTO!.FullName!, userDTO.Username!, userDTO.Email!, userDTO.Password)));

    private static string? MissingFieldMessage(WebUserDto user) =>
        string.IsNullOrWhiteSpace(user.Email) ? EmailRequiredMessage
        : string.IsNullOrWhiteSpace(user.FullName) ? FullNameRequiredMessage
        : string.IsNullOrWhiteSpace(user.Username) ? UsernameRequiredMessage
        : null;

    private static IResult ToResult(CreateAccountResult result) => result.Status switch
    {
        CreateAccountStatus.Created => Results.Ok(new { message = CreatedMessage, username = result.UserName, email = result.Email }),
        CreateAccountStatus.EmailInUse => MvcResults.Message(StatusCodes.Status400BadRequest, EmailInUseMessage),
        _ => Results.BadRequest(result.Errors)
    };
}

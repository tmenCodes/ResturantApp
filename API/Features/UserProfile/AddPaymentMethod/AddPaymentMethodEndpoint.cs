using System.Security.Claims;
using API.Common;
using API.Features.UserProfile.Shared;
using Microsoft.AspNetCore.Mvc;

namespace API.Features.UserProfile.AddPaymentMethod;

/// <summary><c>POST api/UserInformation/AddUserPaymentInformation</c>: saves a payment method for the signed-in user.</summary>
public class AddPaymentMethodEndpoint : IEndpoint
{
    public const string Route = "AddUserPaymentInformation";

    public void Map(IEndpointRouteBuilder app) =>
        ApiGroups.UserInformation(app).MapPost(Route, async (
            [FromBody] UserPaymentInformationDto? userPaymentInformationdto, ClaimsPrincipal user,
            AddPaymentMethodHandler handler, ILogger<AddPaymentMethodEndpoint> logger) =>
        {
            if (user.Identity?.Name is not { } userName)
            {
                return MvcResults.Problem(StatusCodes.Status400BadRequest);
            }

            logger.LogInformation("User {UserName} has logged in.", userName);
            return Results.Ok(await handler.HandleAsync(userName, userPaymentInformationdto!));
        }).RequireAuthorization();
}

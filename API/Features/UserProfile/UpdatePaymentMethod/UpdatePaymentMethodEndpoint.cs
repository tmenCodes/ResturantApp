using System.Security.Claims;
using API.Common;
using API.Features.UserProfile.Shared;
using Microsoft.AspNetCore.Mvc;

namespace API.Features.UserProfile.UpdatePaymentMethod;

/// <summary><c>PUT api/UserInformation/UpdateUserPaymentInformation</c>: replaces the signed-in user's payment method.</summary>
public class UpdatePaymentMethodEndpoint : IEndpoint
{
    public const string Route = "UpdateUserPaymentInformation";

    public void Map(IEndpointRouteBuilder app) =>
        ApiGroups.UserInformation(app).MapPut(Route, async (
            [FromBody] UserPaymentInformationDto? userPaymentInformationdto, ClaimsPrincipal user, UpdatePaymentMethodHandler handler) =>
            user.Identity?.Name is { } userName && await handler.HandleAsync(userName, userPaymentInformationdto!) is { } updated
                ? Results.Ok(updated)
                : MvcResults.Problem(StatusCodes.Status400BadRequest))
        .RequireAuthorization();
}

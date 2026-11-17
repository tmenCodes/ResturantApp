using System.Security.Claims;
using API.Common;

namespace API.Features.UserProfile.GetPaymentMethod;

/// <summary><c>GET api/UserInformation/GetPaymentMethod</c>: reads the signed-in user's payment method; 204 when none.</summary>
public class GetPaymentMethodEndpoint : IEndpoint
{
    public const string Route = "GetPaymentMethod";

    public void Map(IEndpointRouteBuilder app) =>
        ApiGroups.UserInformation(app).MapGet(Route, async (ClaimsPrincipal user, GetPaymentMethodHandler handler) =>
            user.Identity?.Name is { } userName
                ? MvcResults.OkOrNoContent(await handler.HandleAsync(userName))
                : MvcResults.Problem(StatusCodes.Status400BadRequest))
        .RequireAuthorization();
}

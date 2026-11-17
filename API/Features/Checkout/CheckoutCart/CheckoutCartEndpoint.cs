using System.Security.Claims;
using API.Common;
using Microsoft.AspNetCore.Mvc;

namespace API.Features.Checkout.CheckoutCart;

/// <summary>
/// <c>POST api/Cart/Checkout</c>: pays for a cart. The order is saved for the signed-in user when a valid
/// token is sent, and as a guest order otherwise.
/// </summary>
public class CheckoutCartEndpoint : IEndpoint
{
    public const string Route = "Checkout";

    private const string AlreadyCheckedOutMessage = "Another pre-existing user has that guid";
    private const string NotFoundMessage = "Invalid guid";

    public void Map(IEndpointRouteBuilder app) =>
        ApiGroups.Cart(app).MapPost(Route, async (
            [FromBody] CheckoutRequestDto? customerInformationDto, ClaimsPrincipal user, CheckoutCartHandler handler,
            ILogger<CheckoutCartEndpoint> logger, CancellationToken cancellationToken) =>
        {
            logger.LogCritical(customerInformationDto!.CartsIdentity);
            var buyer = user.Identity?.IsAuthenticated == true ? user.Identity.Name : null;
            var result = await handler.HandleAsync(customerInformationDto, buyer, cancellationToken);
            return result.Status switch
            {
                CheckoutStatus.Created => Results.Ok(result.Checkout),
                CheckoutStatus.CartAlreadyCheckedOut => MvcResults.Message(StatusCodes.Status400BadRequest, AlreadyCheckedOutMessage),
                _ => MvcResults.Message(StatusCodes.Status400BadRequest, NotFoundMessage)
            };
        });
}

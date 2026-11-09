using API.Common;
using Microsoft.AspNetCore.Mvc;

namespace API.Features.Checkout.CheckoutCart;

/// <summary><c>POST api/Payment/PaymentInformation</c>: pays for a cart.</summary>
public class CheckoutCartEndpoint : IEndpoint
{
    public const string Route = "PaymentInformation";

    private const string AlreadyCheckedOutMessage = "Another pre-existing user has that guid";
    private const string NotFoundMessage = "Invalid guid";

    public void Map(IEndpointRouteBuilder app) =>
        ApiGroups.Payment(app).MapPost(Route, async (
            [FromBody] CheckoutRequestDto? customerInformationDto, CheckoutCartHandler handler,
            ILogger<CheckoutCartEndpoint> logger, CancellationToken cancellationToken) =>
        {
            logger.LogCritical(customerInformationDto!.CartsIdentity);
            var result = await handler.HandleAsync(customerInformationDto, cancellationToken);
            return result.Status switch
            {
                CheckoutStatus.Created => Results.Ok(result.Checkout),
                CheckoutStatus.CartAlreadyCheckedOut => MvcResults.Message(StatusCodes.Status400BadRequest, AlreadyCheckedOutMessage),
                _ => MvcResults.Message(StatusCodes.Status400BadRequest, NotFoundMessage)
            };
        });
}

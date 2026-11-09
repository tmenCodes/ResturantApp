using API.Common;

namespace API.Features.Checkout.GetAllCheckouts;

/// <summary><c>GET api/Payment/GetAllOrders</c>: lists past checkouts.</summary>
public class GetAllCheckoutsEndpoint : IEndpoint
{
    public const string Route = "GetAllOrders";

    private const string NoneMessage = "No orders were found.";

    public void Map(IEndpointRouteBuilder app) =>
        ApiGroups.Payment(app).MapGet(Route, async (GetAllCheckoutsHandler handler, CancellationToken cancellationToken) =>
            await handler.HandleAsync(cancellationToken) is { Count: > 0 } orders
                ? Results.Ok(orders)
                : MvcResults.Message(StatusCodes.Status400BadRequest, NoneMessage));
}

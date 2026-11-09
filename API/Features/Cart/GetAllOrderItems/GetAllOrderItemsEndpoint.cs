using API.Common;

namespace API.Features.Cart.GetAllOrderItems;

/// <summary><c>GET api/Order/GetAllTempItems</c>: lists the order items in every cart.</summary>
public class GetAllOrderItemsEndpoint : IEndpoint
{
    public const string Route = "GetAllTempItems";

    private const string NotFoundMessage = "no value was found";

    public void Map(IEndpointRouteBuilder app) =>
        ApiGroups.Order(app).MapGet(Route, async (GetAllOrderItemsHandler handler) =>
            await handler.HandleAsync() is { Count: > 0 } items
                ? Results.Ok(items)
                : MvcResults.Message(StatusCodes.Status404NotFound, NotFoundMessage));
}

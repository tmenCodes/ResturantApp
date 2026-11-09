using API.Common;

namespace API.Features.Cart.RemoveCartItem;

/// <summary><c>DELETE api/Order/DeleteMenuItem</c>: removes a line from a cart.</summary>
public class RemoveCartItemEndpoint : IEndpoint
{
    public const string Route = "DeleteMenuItem";

    private const string NotFoundMessage = "No menu item was found";

    /// <remarks>Missing parameters bind to their defaults, as MVC bound them, and so match no line.</remarks>
    public void Map(IEndpointRouteBuilder app) =>
        ApiGroups.Order(app).MapDelete(Route, async (
            int? id, Guid? guidId, RemoveCartItemHandler handler, CancellationToken cancellationToken) =>
            await handler.HandleAsync(id ?? default, guidId ?? default, cancellationToken)
                ? Results.Ok()
                : MvcResults.Message(StatusCodes.Status404NotFound, NotFoundMessage));
}

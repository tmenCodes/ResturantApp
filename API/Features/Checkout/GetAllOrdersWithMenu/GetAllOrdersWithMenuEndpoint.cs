using API.Common;

namespace API.Features.Checkout.GetAllOrdersWithMenu;

/// <summary><c>GET api/Payment/GetAllOrdersWithMenu</c>: placeholder for orders with their menu items; returns fixed text.</summary>
public class GetAllOrdersWithMenuEndpoint : IEndpoint
{
    public const string Route = "GetAllOrdersWithMenu";

    private const string PlaceholderMessage = "completed for now";

    public void Map(IEndpointRouteBuilder app) =>
        ApiGroups.Payment(app).MapGet(Route, () => MvcResults.Message(StatusCodes.Status200OK, PlaceholderMessage));
}

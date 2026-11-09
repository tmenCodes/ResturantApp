using System.ComponentModel.DataAnnotations;
using API.Common;

namespace API.Features.Cart.GetCartTotals;

/// <summary><c>GET api/Order/GetTotalPrice</c>: reads a cart's totals.</summary>
public class GetCartTotalsEndpoint : IEndpoint
{
    public const string Route = "GetTotalPrice";

    public void Map(IEndpointRouteBuilder app) =>
        ApiGroups.Order(app).MapGet(Route, async ([Required] string? guid, GetCartTotalsHandler handler) =>
            await handler.HandleAsync(guid!) is { } totals
                ? Results.Ok(totals)
                : MvcResults.Problem(StatusCodes.Status404NotFound));
}

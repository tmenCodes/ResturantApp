using System.ComponentModel.DataAnnotations;
using API.Common;

namespace API.Features.Cart.GetCartTotals;

/// <summary><c>GET api/Cart/GetTotals</c>: reads a cart's totals.</summary>
public class GetCartTotalsEndpoint : IEndpoint
{
    public const string Route = "GetTotals";

    public void Map(IEndpointRouteBuilder app) =>
        ApiGroups.Cart(app).MapGet(Route, async ([Required] string? guid, GetCartTotalsHandler handler) =>
            await handler.HandleAsync(guid!) is { } totals
                ? Results.Ok(totals)
                : MvcResults.Problem(StatusCodes.Status404NotFound));
}

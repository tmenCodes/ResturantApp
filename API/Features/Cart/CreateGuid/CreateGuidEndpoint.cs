using API.Common;

namespace API.Features.Cart.CreateGuid;

/// <summary><c>GET api/Cart/CreateCart</c>: returns a new GUID the frontend uses as its cart identity.</summary>
public class CreateGuidEndpoint : IEndpoint
{
    public const string Route = "CreateCart";

    public void Map(IEndpointRouteBuilder app) =>
        ApiGroups.Cart(app).MapGet(Route, () => Results.Ok(Guid.NewGuid()));
}

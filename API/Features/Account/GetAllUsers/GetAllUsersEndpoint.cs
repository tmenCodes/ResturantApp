using API.Common;

namespace API.Features.Account.GetAllUsers;

/// <summary><c>GET api/Admin/ReturnAllUsers</c>: lists registered accounts. Requires the SuperAdmin role (set on the group).</summary>
public class GetAllUsersEndpoint : IEndpoint
{
    public const string Route = "ReturnAllUsers";

    public void Map(IEndpointRouteBuilder app) =>
        ApiGroups.Admin(app).MapGet(Route, async (GetAllUsersHandler handler) => Results.Ok(await handler.HandleAsync()));
}

using API.Common;

namespace API.Features.Account.GetAllUsers;

/// <summary><c>GET api/RegisterUser/ReturnAllUsers</c>: lists registered accounts. Requires sign-in.</summary>
public class GetAllUsersEndpoint : IEndpoint
{
    public const string Route = "ReturnAllUsers";

    public void Map(IEndpointRouteBuilder app) =>
        ApiGroups.RegisterUser(app).MapGet(Route, async (GetAllUsersHandler handler) => Results.Ok(await handler.HandleAsync()))
            .RequireAuthorization();
}

using System.ComponentModel.DataAnnotations;
using API.Common;

namespace API.Features.Account.FindByUser;

/// <summary><c>GET api/RegisterUser/FindByUser</c>: whether a user name is already registered.</summary>
public class FindByUserEndpoint : IEndpoint
{
    public const string Route = "FindByUser";

    public void Map(IEndpointRouteBuilder app) =>
        ApiGroups.RegisterUser(app).MapGet(Route, async ([Required] string? username, UserNameExistsHandler handler) =>
            Results.Ok(await handler.HandleAsync(username)));
}

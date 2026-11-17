using System.ComponentModel.DataAnnotations;
using API.Common;

namespace API.Features.Account.CheckDuplicateEmail;

/// <summary><c>GET api/Registration/CheckDuplicateEmail</c>: whether an email is already registered.</summary>
public class CheckDuplicateEmailEndpoint : IEndpoint
{
    public const string Route = "CheckDuplicateEmail";

    public void Map(IEndpointRouteBuilder app) =>
        ApiGroups.Registration(app).MapGet(Route, async ([Required] string? email, EmailExistsHandler handler) =>
            Results.Ok(await handler.HandleAsync(email!)));
}

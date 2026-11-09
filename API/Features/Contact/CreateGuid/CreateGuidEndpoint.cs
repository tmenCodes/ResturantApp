using API.Common;

namespace API.Features.Contact.CreateGuid;

/// <summary><c>GET api/Misc/CreateGuideForFrontend</c>: returns a new GUID the frontend uses as its cart identity.</summary>
public class CreateGuidEndpoint : IEndpoint
{
    public const string Route = "CreateGuideForFrontend";

    public void Map(IEndpointRouteBuilder app) =>
        ApiGroups.Misc(app).MapGet(Route, () => Results.Ok(Guid.NewGuid()));
}

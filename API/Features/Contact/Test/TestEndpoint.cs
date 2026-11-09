using API.Common;

namespace API.Features.Contact.Test;

/// <summary><c>GET api/Misc/Test</c>: a liveness check that returns the text "Test".</summary>
public class TestEndpoint : IEndpoint
{
    public const string Route = "Test";

    private const string Response = "Test";

    public void Map(IEndpointRouteBuilder app) =>
        ApiGroups.Misc(app).MapGet(Route, () => Results.Text(Response));
}

using API.Common;
using Microsoft.AspNetCore.Mvc;

namespace API.Features.Contact.CreateInquiry;

/// <summary><c>POST api/Misc/CustomerInquiry</c>: submits the contact form.</summary>
public class CreateInquiryEndpoint : IEndpoint
{
    public const string Route = "CustomerInquiry";

    public void Map(IEndpointRouteBuilder app) =>
        ApiGroups.Misc(app).MapPost(Route, async (
            [FromBody] CustomerInquiryDto? contact, CreateInquiryHandler handler, CancellationToken cancellationToken) =>
            Results.Ok(await handler.HandleAsync(contact!, cancellationToken)));
}

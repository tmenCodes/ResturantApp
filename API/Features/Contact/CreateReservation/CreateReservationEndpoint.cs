using API.Common;
using Microsoft.AspNetCore.Mvc;

namespace API.Features.Contact.CreateReservation;

/// <summary><c>POST api/Contact/MakeReservation</c>: books a table.</summary>
public class CreateReservationEndpoint : IEndpoint
{
    public const string Route = "MakeReservation";

    public void Map(IEndpointRouteBuilder app) =>
        ApiGroups.Contact(app).MapPost(Route, async (
            [FromBody] ReservationDto? bookingInformation, CreateReservationHandler handler, CancellationToken cancellationToken) =>
            Results.Ok(await handler.HandleAsync(bookingInformation!, cancellationToken)));
}

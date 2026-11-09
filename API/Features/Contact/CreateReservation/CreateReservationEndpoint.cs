using API.Common;
using Microsoft.AspNetCore.Mvc;

namespace API.Features.Contact.CreateReservation;

/// <summary><c>POST api/Misc/BookingInformation</c>: books a table.</summary>
public class CreateReservationEndpoint : IEndpoint
{
    public const string Route = "BookingInformation";

    public void Map(IEndpointRouteBuilder app) =>
        ApiGroups.Misc(app).MapPost(Route, async (
            [FromBody] ReservationDto? bookingInformation, CreateReservationHandler handler, CancellationToken cancellationToken) =>
            Results.Ok(await handler.HandleAsync(bookingInformation!, cancellationToken)));
}

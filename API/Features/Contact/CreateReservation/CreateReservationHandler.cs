using API.Common;
using Resturant.Domain.ContactInfo;
using Resturant.Infrastructure.Persistence;

namespace API.Features.Contact.CreateReservation;

/// <summary>Saves a table reservation.</summary>
public class CreateReservationHandler : IHandler
{
    private readonly RestaurantContext _context;

    public CreateReservationHandler(RestaurantContext context)
    {
        _context = context;
    }

    /// <summary>Saves a reservation. Any client-supplied id is ignored; the returned DTO carries the generated id.</summary>
    public async Task<ReservationDto> HandleAsync(ReservationDto reservation, CancellationToken cancellationToken = default)
    {
        var entity = new Reservation(
            reservation.Name, reservation.Email, reservation.Phone, reservation.Date, reservation.Time,
            reservation.People, reservation.Message);
        await _context.Reservations.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return ToDto(entity);
    }

    private static ReservationDto ToDto(Reservation entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Email = entity.Email,
        Phone = entity.Phone,
        Date = entity.Date,
        Time = entity.Time,
        People = entity.People,
        Message = entity.Message
    };
}

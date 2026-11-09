using API.Common;
using Resturant.Domain.Contact;
using Resturant.Domain.Repositories;

namespace API.Features.Contact.CreateReservation;

/// <summary>Saves a table reservation.</summary>
public class CreateReservationHandler : IHandler
{
    private readonly IContactRepository _contactRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateReservationHandler(IContactRepository contactRepository, IUnitOfWork unitOfWork)
    {
        _contactRepository = contactRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>Saves a reservation. Any client-supplied id is ignored; the returned DTO carries the generated id.</summary>
    public async Task<ReservationDto> HandleAsync(ReservationDto reservation, CancellationToken cancellationToken = default)
    {
        var entity = new Reservation(
            reservation.Name, reservation.Email, reservation.Phone, reservation.Date, reservation.Time,
            reservation.People, reservation.Message);
        await _contactRepository.AddReservationAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
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

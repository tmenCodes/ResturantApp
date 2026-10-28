using Resturant.Business.Dtos;
using Resturant.Data.Entities;
using Resturant.Data.Repositories;

namespace Resturant.Business.Services;

/// <summary>Default <see cref="IContactService"/> backed by <see cref="IContactRepository"/>.</summary>
public class ContactService : IContactService
{
    private readonly IContactRepository _contactRepository;

    public ContactService(IContactRepository contactRepository)
    {
        _contactRepository = contactRepository;
    }

    public async Task<ReservationDto> CreateReservationAsync(ReservationDto reservation, CancellationToken cancellationToken = default)
    {
        var entity = new Reservation
        {
            Name = reservation.Name,
            Email = reservation.Email,
            Phone = reservation.Phone,
            Date = reservation.Date,
            Time = reservation.Time,
            People = reservation.People,
            Message = reservation.Message
        };
        await _contactRepository.AddReservationAsync(entity, cancellationToken);
        return ToDto(entity);
    }

    public async Task<CustomerInquiryDto> CreateInquiryAsync(CustomerInquiryDto inquiry, CancellationToken cancellationToken = default)
    {
        var entity = new CustomerInquiry
        {
            Name = inquiry.Name,
            Email = inquiry.Email,
            Subject = inquiry.Subject,
            Message = inquiry.Message
        };
        await _contactRepository.AddInquiryAsync(entity, cancellationToken);
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

    private static CustomerInquiryDto ToDto(CustomerInquiry entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Email = entity.Email,
        Subject = entity.Subject,
        Message = entity.Message
    };
}

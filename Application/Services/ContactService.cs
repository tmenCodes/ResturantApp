using Resturant.Application.Dtos;
using Resturant.Domain.Contact;
using Resturant.Domain.Repositories;
using Resturant.Application.Interfaces;

namespace Resturant.Application.Services;

/// <summary>Default <see cref="IContactService"/> backed by <see cref="IContactRepository"/>.</summary>
public class ContactService : IContactService
{
    private readonly IContactRepository _contactRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ContactService(IContactRepository contactRepository, IUnitOfWork unitOfWork)
    {
        _contactRepository = contactRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ReservationDto> CreateReservationAsync(ReservationDto reservation, CancellationToken cancellationToken = default)
    {
        var entity = new Reservation(
            reservation.Name, reservation.Email, reservation.Phone, reservation.Date, reservation.Time,
            reservation.People, reservation.Message);
        await _contactRepository.AddReservationAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ToDto(entity);
    }

    public async Task<CustomerInquiryDto> CreateInquiryAsync(CustomerInquiryDto inquiry, CancellationToken cancellationToken = default)
    {
        var entity = new CustomerInquiry(inquiry.Name, inquiry.Email, inquiry.Subject, inquiry.Message);
        await _contactRepository.AddInquiryAsync(entity, cancellationToken);
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

    private static CustomerInquiryDto ToDto(CustomerInquiry entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Email = entity.Email,
        Subject = entity.Subject,
        Message = entity.Message
    };
}

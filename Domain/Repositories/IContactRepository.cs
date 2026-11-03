using Resturant.Domain.Contact;
namespace Resturant.Domain.Repositories;

/// <summary>Persists messages customers send to the restaurant: table reservations and contact inquiries.</summary>
public interface IContactRepository
{
    /// <summary>Stages a new reservation; <see cref="IUnitOfWork.SaveChangesAsync"/> persists it and assigns its <see cref="Reservation.Id"/>.</summary>
    Task AddReservationAsync(Reservation reservation, CancellationToken cancellationToken = default);

    /// <summary>Stages a new inquiry; <see cref="IUnitOfWork.SaveChangesAsync"/> persists it and assigns its <see cref="CustomerInquiry.Id"/>.</summary>
    Task AddInquiryAsync(CustomerInquiry inquiry, CancellationToken cancellationToken = default);
}

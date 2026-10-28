using Resturant.Data.Entities;

namespace Resturant.Data.Repositories;

/// <summary>Persists messages customers send to the restaurant: table reservations and contact inquiries.</summary>
public interface IContactRepository
{
    /// <summary>Saves a new reservation and assigns its <see cref="Reservation.Id"/>.</summary>
    Task AddReservationAsync(Reservation reservation, CancellationToken cancellationToken = default);

    /// <summary>Saves a new inquiry and assigns its <see cref="CustomerInquiry.Id"/>.</summary>
    Task AddInquiryAsync(CustomerInquiry inquiry, CancellationToken cancellationToken = default);
}

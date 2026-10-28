using Resturant.Business.Dtos;

namespace Resturant.Business.Services;

/// <summary>Handles reservations and contact inquiries submitted by customers.</summary>
public interface IContactService
{
    /// <summary>Saves a reservation. Any client-supplied id is ignored; the returned DTO carries the generated id.</summary>
    Task<ReservationDto> CreateReservationAsync(ReservationDto reservation, CancellationToken cancellationToken = default);

    /// <summary>Saves an inquiry. Any client-supplied id is ignored; the returned DTO carries the generated id.</summary>
    Task<CustomerInquiryDto> CreateInquiryAsync(CustomerInquiryDto inquiry, CancellationToken cancellationToken = default);
}

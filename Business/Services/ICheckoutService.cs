using Resturant.Business.Dtos;

namespace Resturant.Business.Services;

/// <summary>Records payment for shopping carts and lists past checkouts.</summary>
public interface ICheckoutService
{
    /// <summary>Returns every checkout, or an empty list when there are none.</summary>
    Task<IReadOnlyList<CheckoutDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks out the cart named by <see cref="CustomerInformationDTO.CartsIdentity"/>. The cart must exist and must
    /// not already be checked out. The checkout is recorded as paid at the current UTC time.
    /// </summary>
    Task<CheckoutResult> CreateAsync(CustomerInformationDTO request, CancellationToken cancellationToken = default);
}

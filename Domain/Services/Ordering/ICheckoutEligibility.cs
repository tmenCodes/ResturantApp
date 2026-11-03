namespace Resturant.Domain.Services.Ordering;

/// <summary>Decides whether a shopping cart may be checked out.</summary>
public interface ICheckoutEligibility
{
    /// <summary>
    /// Returns <see cref="CheckoutEligibilityResult.Eligible"/> when the cart exists and has not been checked out yet.
    /// An already checked-out cart is reported as such even if the cart itself no longer exists.
    /// </summary>
    Task<CheckoutEligibilityResult> CheckAsync(Guid cartIdentity, CancellationToken cancellationToken = default);
}

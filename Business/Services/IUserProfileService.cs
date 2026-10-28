using Resturant.Business.Dtos;

namespace Resturant.Business.Services;

/// <summary>Manages a signed-in user's saved payment method and reads their order history.</summary>
public interface IUserProfileService
{
    /// <summary>Saves a new payment method for the user and returns it.</summary>
    Task<SavedPaymentMethodDto> AddPaymentMethodAsync(string userName, UserPaymentInformationDto paymentInformation, CancellationToken cancellationToken = default);

    /// <summary>Returns the user's saved payment method, or null when they have none.</summary>
    Task<SavedPaymentMethodDto?> GetPaymentMethodAsync(string userName, CancellationToken cancellationToken = default);

    /// <summary>Overwrites the user's saved payment method. Returns null when they have none to update.</summary>
    Task<SavedPaymentMethodDto?> UpdatePaymentMethodAsync(string userName, UserPaymentInformationDto paymentInformation, CancellationToken cancellationToken = default);

    /// <summary>Returns up to five of the user's past order totals, ordered by their short-date text.</summary>
    Task<IReadOnlyList<UserOrderHistoryDto>> GetRecentOrderTotalsAsync(string userName, CancellationToken cancellationToken = default);

    /// <summary>Returns every item from every order the user has checked out.</summary>
    Task<IReadOnlyList<UserOrderedItemDto>> GetOrderedItemsAsync(string userName, CancellationToken cancellationToken = default);
}

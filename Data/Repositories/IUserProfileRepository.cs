using Resturant.Data.Entities;

namespace Resturant.Data.Repositories;

/// <summary>Reads and writes a user's saved payment method and order history.</summary>
public interface IUserProfileRepository
{
    /// <summary>Returns the user's first saved payment method (lowest id), or null when they have none.</summary>
    Task<SavedPaymentMethod?> GetPaymentMethodAsync(string userName, CancellationToken cancellationToken = default);

    /// <summary>Saves a new payment method and assigns its <see cref="SavedPaymentMethod.Id"/>.</summary>
    Task AddPaymentMethodAsync(SavedPaymentMethod paymentMethod, CancellationToken cancellationToken = default);

    /// <summary>Saves changes made to an existing payment method.</summary>
    Task UpdatePaymentMethodAsync(SavedPaymentMethod paymentMethod, CancellationToken cancellationToken = default);

    /// <summary>Returns the shopping carts the user has checked out, one per checkout.</summary>
    Task<List<ShoppingCart>> GetCheckedOutCartsAsync(string userName, CancellationToken cancellationToken = default);

    /// <summary>Returns every item in the shopping carts the user has checked out.</summary>
    Task<List<CheckedOutOrderItem>> GetCheckedOutOrderItemsAsync(string userName, CancellationToken cancellationToken = default);
}

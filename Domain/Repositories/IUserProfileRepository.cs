using Resturant.Domain.Customers;
using Resturant.Domain.Ordering;
namespace Resturant.Domain.Repositories;

/// <summary>Reads and writes a user's saved payment method and order history.</summary>
public interface IUserProfileRepository
{
    /// <summary>
    /// Returns the user's first saved payment method (lowest id), or null when they have none. The result is tracked,
    /// so changes to it are persisted by <see cref="IUnitOfWork.SaveChangesAsync"/>.
    /// </summary>
    Task<SavedPaymentMethod?> GetPaymentMethodAsync(string userName, CancellationToken cancellationToken = default);

    /// <summary>Stages a new payment method; <see cref="IUnitOfWork.SaveChangesAsync"/> persists it and assigns its <see cref="SavedPaymentMethod.Id"/>.</summary>
    Task AddPaymentMethodAsync(SavedPaymentMethod paymentMethod, CancellationToken cancellationToken = default);

    /// <summary>Returns the shopping carts the user has checked out, one per checkout.</summary>
    Task<List<ShoppingCart>> GetCheckedOutCartsAsync(string userName, CancellationToken cancellationToken = default);

    /// <summary>Returns every item in the shopping carts the user has checked out.</summary>
    Task<List<CheckedOutOrderItem>> GetCheckedOutOrderItemsAsync(string userName, CancellationToken cancellationToken = default);
}

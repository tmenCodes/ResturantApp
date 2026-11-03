namespace Resturant.Application.Abstractions;

/// <summary>Read-only queries over a user's past orders. Returns read models, not domain entities.</summary>
public interface IOrderHistoryQueries
{
    /// <summary>Returns the shopping carts the user has checked out, one per checkout.</summary>
    Task<List<CheckedOutCart>> GetCheckedOutCartsAsync(string userName, CancellationToken cancellationToken = default);

    /// <summary>Returns every item in the shopping carts the user has checked out.</summary>
    Task<List<CheckedOutOrderItem>> GetCheckedOutOrderItemsAsync(string userName, CancellationToken cancellationToken = default);
}

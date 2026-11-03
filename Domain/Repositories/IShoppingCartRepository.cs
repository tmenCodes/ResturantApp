using Resturant.Domain.Ordering;

namespace Resturant.Domain.Repositories;

/// <summary>Reads and writes shopping carts and their order items.</summary>
public interface IShoppingCartRepository
{
    /// <summary>
    /// Returns the tracked cart, with its order items, whose identity as a lowercase GUID string equals
    /// <paramref name="cartIdentity"/>, or null.
    /// </summary>
    Task<ShoppingCart?> GetCartAsync(string cartIdentity);

    /// <summary>Persists all pending changes made through this repository.</summary>
    Task<int> SaveChangesAsync();

    /// <summary>Returns every order item across all carts.</summary>
    Task<List<OrderItem>> GetAllOrderItemsAsync();

    /// <summary>Returns the order items, with their options, of the cart whose identity string equals <paramref name="cartIdentity"/>.</summary>
    Task<List<OrderItem>> GetCartOrderItemsAsync(string cartIdentity);

    /// <summary>Adds a new cart; call <see cref="SaveChangesAsync"/> to persist.</summary>
    Task AddCartAsync(ShoppingCart cart);
}

using Resturant.Domain.Ordering;

namespace Resturant.Domain.Repositories;

/// <summary>Reads and writes shopping carts and their order items.</summary>
public interface IShoppingCartRepository
{
    /// <summary>Returns the tracked cart, with its order items, that has this identity, or null.</summary>
    Task<ShoppingCart?> GetCartAsync(Guid cartIdentity);

    /// <summary>Returns every order item across all carts.</summary>
    Task<List<OrderItem>> GetAllOrderItemsAsync();

    /// <summary>Returns the order items, with their options, of the cart with this identity.</summary>
    Task<List<OrderItem>> GetCartOrderItemsAsync(Guid cartIdentity);

    /// <summary>Stages a new cart; <see cref="IUnitOfWork.SaveChangesAsync"/> persists it.</summary>
    Task AddCartAsync(ShoppingCart cart);
}

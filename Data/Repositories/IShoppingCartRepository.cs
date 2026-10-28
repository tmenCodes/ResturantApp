using Resturant.Data.Entities;

namespace Resturant.Data.Repositories
{

    /// <summary>Reads and writes shopping carts and their order items.</summary>
    public interface IShoppingCartRepository
    {
        /// <summary>Returns the tracked cart whose identity, as a lowercase GUID string, equals <paramref name="cartIdentity"/>, or null.</summary>
        Task<ShoppingCart?> GetCartAsync(string cartIdentity);

        /// <summary>Persists all pending changes made through this repository.</summary>
        Task<int> SaveChangesAsync();

        /// <summary>Returns the order item with this id, or null.</summary>
        Task<OrderItem?> GetOrderItemAsync(int orderItemId);

        /// <summary>Returns every order item across all carts.</summary>
        Task<List<OrderItem>> GetAllOrderItemsAsync();

        /// <summary>Returns the order items, with their options, of the cart whose identity string equals <paramref name="cartIdentity"/>.</summary>
        Task<List<OrderItem>> GetCartOrderItemsAsync(string cartIdentity);

        /// <summary>Adds a new cart; call <see cref="SaveChangesAsync"/> to persist.</summary>
        Task AddCartAsync(ShoppingCart cart);

        /// <summary>Marks an order item for deletion; call <see cref="SaveChangesAsync"/> to persist.</summary>
        void RemoveOrderItem(OrderItem orderItem);

        /// <summary>Returns the sum of the saved order item totals in the cart.</summary>
        decimal GetCartSubTotal(Guid cartIdentity);


    }
}

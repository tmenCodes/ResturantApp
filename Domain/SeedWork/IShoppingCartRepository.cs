using Resturant.Domain.Entity;

namespace Resturant.Domain.SeedWork
{

    /// <summary>Reads and writes shopping carts and their order items.</summary>
    public interface IShoppingCartRepository
    {
        Task<ShoppingCart?> ReturnCartItemsByGuidAsync(string guid);

        Task<int> SaveCartItemsAsync();

        Task<OrderItem?> FindByPrimaryKey(int id);

        Task<List<OrderItem>> ReturnMenuItemsListAsync();
        
        Task<List<OrderItem>> ReturnMenuItemListByGuid(string guidId);

        Task AddShoppingCartItem(ShoppingCart shoppingCartItem);

        decimal SubTotalMenuPrice(Guid menuGuid);


    }
}
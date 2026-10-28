using Resturant.Domain.Entity;

namespace Resturant.Domain.SeedWork
{

    public interface IRepository
    {
        Task<List<ShoppingCart>> ReturnListItemsAsync();
        Task<ShoppingCart?> ReturnCartItemsByGuidAsync(string guid);

        Task<int> SaveCartItemsAsync();

        Task<OrderItem?> FindByPrimaryKey(int id);

        Task<List<OrderItem>> ReturnMenuItemsListAsync();
        
        Task<List<OrderItem>> ReturnMenuItemListByGuid(string guidId);

        Task AddShoppingCartItem(ShoppingCart shoppingCartItem);

        decimal SubTotalMenuPrice(Guid menuGuid);


    }
}
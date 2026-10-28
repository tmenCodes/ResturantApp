using Microsoft.EntityFrameworkCore;
using Resturant.Data.Entities;
using Resturant.Data.Repositories;
using Resturant.Data.Context;

namespace Resturant.Data.Repositories;

public class ShoppingCartRepository : IShoppingCartRepository
{
      private readonly RestaurantContext _context; 
   
    public ShoppingCartRepository(RestaurantContext context)
    {
        ArgumentException.ThrowIfNullOrEmpty(nameof(context));
        _context = context; 

    }
    
    public async Task<ShoppingCart?> ReturnCartItemsByGuidAsync(string guid)
    {
        ShoppingCart? tempItemPrice = await _context.
            ShoppingCarts.FirstOrDefaultAsync(x=>x.Identity.ToString() == guid);
        return tempItemPrice ?? null; 
    }
    
    public async Task<OrderItem?> FindByPrimaryKey(int id)
    {     
        var menuItem =  await _context.OrderItem.FindAsync(id); 
        return menuItem;
    }
    
    public async Task<List<OrderItem>> ReturnMenuItemsListAsync()
    {
        //respository pattern should only return primitives---not any dtos
        return await _context.ShoppingCarts
            .Where(x => x.Identity.ToString() != string.Empty)
            .SelectMany(x => x.OrderItems).ToListAsync();
            //.Select(x => new MenuDTO() { Id = x.Id, Name = x.Name, Price = x.Price, GuidId = x.ShoppingCartIdentity.ToString() }).ToListAsync();
            
    }

    public async Task<List<OrderItem>>  ReturnMenuItemListByGuid(string guidId)
    {

        var shoppingCart = await _context.ShoppingCarts
            .Where(x => x.Identity.ToString() == guidId)
            .SelectMany(x => x.OrderItems
            ).Include(x=>x.Options).ToListAsync();

       return shoppingCart;
   
        
    }

    public async Task AddShoppingCartItem(ShoppingCart shoppingCartItem)
    {
        await _context.AddAsync(shoppingCartItem);
    }
    
    
    //calculates all menu time prices
    public decimal SubTotalMenuPrice(Guid menuGuid)
    {
        return _context.ShoppingCarts.Where(x => x.Identity == menuGuid).
            SelectMany(x=>x.OrderItems).
            Sum(x => x.TotalItemPrice);
        
    }
    
    public async Task<int> SaveCartItemsAsync()
    {
        return await _context.SaveChangesAsync();
    }
    
}
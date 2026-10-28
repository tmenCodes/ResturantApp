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
    
    public async Task<ShoppingCart?> GetCartAsync(string cartIdentity)
    {
        ShoppingCart? tempItemPrice = await _context.
            ShoppingCarts.FirstOrDefaultAsync(x=>x.Identity.ToString() == cartIdentity);
        return tempItemPrice ?? null; 
    }
    
    public async Task<OrderItem?> GetOrderItemAsync(int orderItemId)
    {     
        var menuItem =  await _context.OrderItem.FindAsync(orderItemId); 
        return menuItem;
    }
    
    public async Task<List<OrderItem>> GetAllOrderItemsAsync()
    {
        //respository pattern should only return primitives---not any dtos
        return await _context.ShoppingCarts
            .Where(x => x.Identity.ToString() != string.Empty)
            .SelectMany(x => x.OrderItems).ToListAsync();
            //.Select(x => new MenuDTO() { Id = x.Id, Name = x.Name, Price = x.Price, GuidId = x.ShoppingCartIdentity.ToString() }).ToListAsync();
            
    }

    public async Task<List<OrderItem>> GetCartOrderItemsAsync(string cartIdentity)
    {

        var shoppingCart = await _context.ShoppingCarts
            .Where(x => x.Identity.ToString() == cartIdentity)
            .SelectMany(x => x.OrderItems
            ).Include(x=>x.Options).ToListAsync();

       return shoppingCart;
   
        
    }

    public void RemoveOrderItem(OrderItem orderItem)
    {
        _context.OrderItem.Remove(orderItem);
    }

    public async Task AddCartAsync(ShoppingCart cart)
    {
        await _context.AddAsync(cart);
    }
    
    
    //calculates all menu time prices
    public decimal GetCartSubTotal(Guid cartIdentity)
    {
        return _context.ShoppingCarts.Where(x => x.Identity == cartIdentity).
            SelectMany(x=>x.OrderItems).
            Sum(x => x.TotalItemPrice);
        
    }
    
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
    
}
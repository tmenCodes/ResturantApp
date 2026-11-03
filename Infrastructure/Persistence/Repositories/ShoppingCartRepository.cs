using Microsoft.EntityFrameworkCore;
using Resturant.Domain.Repositories;
using Resturant.Domain.Ordering;

namespace Resturant.Infrastructure.Persistence.Repositories;

/// <summary>EF Core adapter for <see cref="IShoppingCartRepository"/>.</summary>
public class ShoppingCartRepository : IShoppingCartRepository
{
    private readonly RestaurantContext _context;

    public ShoppingCartRepository(RestaurantContext context)
    {
        _context = context;
    }

    public Task<ShoppingCart?> GetCartAsync(string cartIdentity)
    {
        return _context.ShoppingCarts
            .Include(x => x.OrderItems)
            .FirstOrDefaultAsync(x => x.Identity.ToString() == cartIdentity);
    }

    public Task<List<OrderItem>> GetAllOrderItemsAsync()
    {
        return _context.ShoppingCarts
            .Where(x => x.Identity.ToString() != string.Empty)
            .SelectMany(x => x.OrderItems)
            .ToListAsync();
    }

    public Task<List<OrderItem>> GetCartOrderItemsAsync(string cartIdentity)
    {
        return _context.ShoppingCarts
            .Where(x => x.Identity.ToString() == cartIdentity)
            .SelectMany(x => x.OrderItems)
            .Include(x => x.Options)
            .ToListAsync();
    }

    public async Task AddCartAsync(ShoppingCart cart)
    {
        await _context.AddAsync(cart);
    }

    public Task<int> SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }
}

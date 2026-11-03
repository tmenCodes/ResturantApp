using Microsoft.EntityFrameworkCore;
using Resturant.Domain.Repositories;
using Resturant.Domain.Ordering;

namespace Resturant.Infrastructure.Persistence.Repositories;

/// <summary>EF Core implementation of <see cref="IShoppingCartRepository"/>.</summary>
public class ShoppingCartRepository : IShoppingCartRepository
{
    private readonly RestaurantContext _context;

    public ShoppingCartRepository(RestaurantContext context)
    {
        _context = context;
    }

    public Task<ShoppingCart?> GetCartAsync(Guid cartIdentity)
    {
        return _context.ShoppingCarts
            .Include(x => x.OrderItems)
            .FirstOrDefaultAsync(x => x.Identity == cartIdentity);
    }

    public Task<List<OrderItem>> GetAllOrderItemsAsync()
    {
        return _context.ShoppingCarts
            .Where(x => x.Identity.ToString() != string.Empty)
            .SelectMany(x => x.OrderItems)
            .ToListAsync();
    }

    public Task<List<OrderItem>> GetCartOrderItemsAsync(Guid cartIdentity)
    {
        return _context.ShoppingCarts
            .Where(x => x.Identity == cartIdentity)
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

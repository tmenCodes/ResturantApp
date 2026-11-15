using API.Common;
using Microsoft.EntityFrameworkCore;
using Resturant.Infrastructure.Persistence;

namespace API.Features.Cart.RemoveCartItem;

/// <summary>Removes a line from a cart.</summary>
public class RemoveCartItemHandler : IHandler
{
    private readonly RestaurantContext _context;

    public RemoveCartItemHandler(RestaurantContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Removes an order item from the cart named by <paramref name="cartIdentity"/> and recalculates its totals.
    /// Returns false when the cart does not exist or does not contain the order item.
    /// </summary>
    public async Task<bool> HandleAsync(int orderItemId, Guid cartIdentity, CancellationToken cancellationToken = default)
    {
        var cart = await _context.ShoppingCarts
            .Include(x => x.OrderItems)
            .FirstOrDefaultAsync(x => x.Identity == cartIdentity, cancellationToken);
        if (cart is null || !cart.RemoveItem(orderItemId))
        {
            return false;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

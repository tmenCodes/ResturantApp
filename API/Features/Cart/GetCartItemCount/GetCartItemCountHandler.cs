using API.Common;
using API.Features.Cart.Shared;
using Microsoft.EntityFrameworkCore;
using Resturant.Infrastructure.Persistence;

namespace API.Features.Cart.GetCartItemCount;

/// <summary>Counts the lines in a cart.</summary>
public class GetCartItemCountHandler : IHandler
{
    private readonly RestaurantContext _context;

    public GetCartItemCountHandler(RestaurantContext context)
    {
        _context = context;
    }

    /// <summary>Returns how many items are in a cart. A malformed or unknown identity yields zero.</summary>
    public Task<int> HandleAsync(string? cartIdentity)
    {
        var identity = CartIdentity.ParseOrEmpty(cartIdentity);
        return _context.ShoppingCarts
            .Where(x => x.Identity == identity)
            .SelectMany(x => x.OrderItems)
            .CountAsync();
    }
}

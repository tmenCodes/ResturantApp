using API.Common;
using Microsoft.EntityFrameworkCore;
using Resturant.Infrastructure.Persistence;

namespace API.Features.UserProfile.GetRecentOrderTotals;

/// <summary>Lists a user's recent order totals.</summary>
public class GetRecentOrderTotalsHandler : IHandler
{
    private const int RecentOrderLimit = 5;

    private readonly RestaurantContext _context;

    public GetRecentOrderTotalsHandler(RestaurantContext context)
    {
        _context = context;
    }

    /// <summary>Returns up to five of the user's past order totals, ordered by their short-date text.</summary>
    public async Task<IReadOnlyList<UserOrderHistoryDto>> HandleAsync(string userName, CancellationToken cancellationToken = default)
    {
        var carts = await (
            from checkout in _context.Orders
            where checkout.UserProfileName == userName
            join cart in _context.ShoppingCarts on checkout.ShoppingCartIdentity equals cart.Identity
            select new { cart.Created, cart.TotalPrice }).ToListAsync(cancellationToken);
        return carts
            .Select(cart => new UserOrderHistoryDto(cart.TotalPrice, cart.Created.ToString("d")))
            .OrderBy(history => history.Date)
            .Take(RecentOrderLimit)
            .ToList();
    }
}

using Microsoft.EntityFrameworkCore;
using Resturant.Application.Abstractions;

namespace Resturant.Infrastructure.Persistence.Queries;

/// <summary>EF Core implementation of <see cref="IOrderHistoryQueries"/>. Joins run in SQL and project to read models.</summary>
public class OrderHistoryQueries : IOrderHistoryQueries
{
    private readonly RestaurantContext _context;

    public OrderHistoryQueries(RestaurantContext context)
    {
        _context = context;
    }

    public Task<List<CheckedOutCart>> GetCheckedOutCartsAsync(string userName, CancellationToken cancellationToken = default)
    {
        var carts =
            from checkout in _context.Checkouts
            where checkout.UserProfileName == userName
            join cart in _context.ShoppingCarts on checkout.ShoppingCartIdentity equals cart.Identity
            select new CheckedOutCart(cart.Created, cart.TotalPrice);
        return carts.ToListAsync(cancellationToken);
    }

    public Task<List<CheckedOutOrderItem>> GetCheckedOutOrderItemsAsync(string userName, CancellationToken cancellationToken = default)
    {
        var items =
            from checkout in _context.Checkouts
            where checkout.UserProfileName == userName
            join item in _context.OrderItem on checkout.ShoppingCartIdentity equals item.ShoppingCartIdentity
            select new CheckedOutOrderItem(checkout.CheckoutTime, checkout.ShoppingCartIdentity, item.Name, item.TotalItemPrice);
        return items.ToListAsync(cancellationToken);
    }
}

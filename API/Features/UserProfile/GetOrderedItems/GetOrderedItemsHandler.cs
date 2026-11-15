using API.Common;
using Microsoft.EntityFrameworkCore;
using Resturant.Infrastructure.Persistence;

namespace API.Features.UserProfile.GetOrderedItems;

/// <summary>Lists the items a user has ordered.</summary>
public class GetOrderedItemsHandler : IHandler
{
    private readonly RestaurantContext _context;

    public GetOrderedItemsHandler(RestaurantContext context)
    {
        _context = context;
    }

    /// <summary>Returns every item from every order the user has checked out.</summary>
    public async Task<IReadOnlyList<UserOrderedItemDto>> HandleAsync(string userName, CancellationToken cancellationToken = default)
    {
        var items = await (
            from checkout in _context.Checkouts
            where checkout.UserProfileName == userName
            join item in _context.OrderItem on checkout.ShoppingCartIdentity equals item.ShoppingCartIdentity
            select new { checkout.CheckoutTime, checkout.ShoppingCartIdentity, item.Name, item.TotalItemPrice })
            .ToListAsync(cancellationToken);
        return items
            .Select(item => new UserOrderedItemDto(
                item.CheckoutTime.ToString(), item.ShoppingCartIdentity.ToString(), item.Name, item.TotalItemPrice.ToString("0.00")))
            .ToList();
    }
}

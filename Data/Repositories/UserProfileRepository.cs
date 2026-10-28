using Microsoft.EntityFrameworkCore;
using Resturant.Data.Context;
using Resturant.Data.Entities;

namespace Resturant.Data.Repositories;

/// <summary>EF Core implementation of <see cref="IUserProfileRepository"/>.</summary>
public class UserProfileRepository : IUserProfileRepository
{
    private readonly RestaurantContext _context;

    public UserProfileRepository(RestaurantContext context)
    {
        _context = context;
    }

    public Task<SavedPaymentMethod?> GetPaymentMethodAsync(string userName, CancellationToken cancellationToken = default)
    {
        return _context.SavedPaymentMethods
            .OrderBy(x => x.Id)
            .FirstOrDefaultAsync(x => x.UserName == userName, cancellationToken);
    }

    public async Task AddPaymentMethodAsync(SavedPaymentMethod paymentMethod, CancellationToken cancellationToken = default)
    {
        await _context.SavedPaymentMethods.AddAsync(paymentMethod, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdatePaymentMethodAsync(SavedPaymentMethod paymentMethod, CancellationToken cancellationToken = default)
    {
        _context.SavedPaymentMethods.Update(paymentMethod);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task<List<ShoppingCart>> GetCheckedOutCartsAsync(string userName, CancellationToken cancellationToken = default)
    {
        var carts =
            from checkout in _context.Checkouts
            where checkout.UserProfileName == userName
            join cart in _context.ShoppingCarts on checkout.ShoppingCartIdentity equals cart.Identity
            select cart;
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

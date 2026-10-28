using Microsoft.EntityFrameworkCore;
using Resturant.Data.Context;
using Resturant.Data.Entities;

namespace Resturant.Data.Repositories;

/// <summary>EF Core implementation of <see cref="ICheckoutRepository"/>.</summary>
public class CheckoutRepository : ICheckoutRepository
{
    private readonly RestaurantContext _context;

    public CheckoutRepository(RestaurantContext context)
    {
        _context = context;
    }

    public Task<List<Checkout>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _context.Checkouts.ToListAsync(cancellationToken);
    }

    public Task<bool> ExistsForCartAsync(Guid shoppingCartIdentity, CancellationToken cancellationToken = default)
    {
        return _context.Checkouts.AnyAsync(x => x.ShoppingCartIdentity == shoppingCartIdentity, cancellationToken);
    }

    public async Task AddAsync(Checkout checkout, CancellationToken cancellationToken = default)
    {
        await _context.Checkouts.AddAsync(checkout, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

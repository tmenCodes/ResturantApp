using Microsoft.EntityFrameworkCore;
using Resturant.Infrastructure.Persistence;
using Resturant.Domain.Customers;
using Resturant.Domain.Repositories;

namespace Resturant.Infrastructure.Persistence.Repositories;

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
    }
}

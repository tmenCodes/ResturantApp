using Resturant.Domain.Repositories;

namespace Resturant.Infrastructure.Persistence;

/// <summary>EF Core implementation of <see cref="IUnitOfWork"/> over the scoped <see cref="RestaurantContext"/>.</summary>
public class EfUnitOfWork : IUnitOfWork
{
    private readonly RestaurantContext _context;

    public EfUnitOfWork(RestaurantContext context)
    {
        _context = context;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
